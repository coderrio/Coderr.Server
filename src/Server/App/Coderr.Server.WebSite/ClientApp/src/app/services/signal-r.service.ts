import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { Subject, Observable } from 'rxjs';
declare type CallbackFilter = (evt: IHubEvent) => boolean;
declare type Action = () => void;
declare type Action1<T> = (arg: T) => void;

export interface IHubEvent {
  typeName: string;
  body: any;
  correlationId: string;
}


interface ICallback {
  filter: CallbackFilter;
  resolve: Action1<IHubEvent>;
  reject: Action1<Error>;
  completed: boolean;
}

interface ICachedMessage {
  receivedAtUtc: Date;
  message: IHubEvent;
}

export interface ISubscriber {
  handle(event: IHubEvent): void;
}

interface ISubscriberWrapper {
  subscriber: ISubscriber;
  filter: CallbackFilter;
}

@Injectable({
  providedIn: "root"
})
export class SignalRService {
  observer: Subject<IHubEvent> = new Subject<IHubEvent>();
  private hubConnection: signalR.HubConnection;
  private waitFilters: ICallback[] = [];
  private cachedMessages: ICachedMessage[] = [];
  private subscribers: ISubscriberWrapper[] = [];

  get eventStream(): Observable<IHubEvent> {
    return this.observer;
  }

  subscribe(filter: CallbackFilter, subscriber: ISubscriber) {
    this.subscribers.push({
      subscriber: subscriber,
      filter: filter,
    });
  }

  unsubscribe(subscriber: ISubscriber) {
    this.subscribers = this.subscribers.filter(x => x.subscriber !== subscriber);
  }

  async wait(filter: CallbackFilter): Promise<IHubEvent> {
    const msg = this.findCachedMessage(filter);
    if (msg != null) {
      return msg;
    }

    var cb: ICallback = {
      filter: filter,
      reject: null,
      resolve: null,
      completed: false
    };
    console.log('got new filter', cb);
    this.waitFilters.push(cb);

    return new Promise((accept, reject) => {
      cb.reject = x => {
        cb.completed = true;
        reject(x);
      };

      cb.resolve = evt => {
        console.log('triggering new filter', evt);
        cb.completed = true;
        accept(evt);
      };

      setTimeout(x => {
        if (cb.completed) {
          return;
        }
        cb.reject(new Error("Timeout"));
        this.waitFilters = this.waitFilters.filter(y => cb !== y);
      },
        5000);
    });
  }

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/hub")
      .build();
    this.hubConnection
      .start()
      .then(() => console.log("Connection started"))
      .catch(err => console.log(`Error while starting connection: ${err}`));

    this.hubConnection.on("OnEvent", (message: IHubEvent) => {
      this.cachedMessages.push({ receivedAtUtc: new Date(), message: message });

      this.resolveFilters(message);

      this
        .subscribers
        .filter(x => x.filter(message))
        .forEach(x => x.subscriber.handle(message));

      this.observer.next(message);
    });
  };

  private findCachedMessage(filter: CallbackFilter) {
    let matchingCachedMessage: ICachedMessage = null;

    var msgsToRemove: ICachedMessage[] = [];
    this.cachedMessages.every(msgWrapper => {
      var msDiff = new Date().getTime() - msgWrapper.receivedAtUtc.getTime();
      if (msDiff > 500) {
        msgsToRemove.push(msgWrapper);
        return true;
      }

      if (filter(msgWrapper.message)) {
        matchingCachedMessage = msgWrapper;
        return false;
      }

      return true;
    });

    msgsToRemove.forEach(msg => {
      this.cachedMessages.filter(x => x !== msg);
    });

    if (matchingCachedMessage != null) {
      return matchingCachedMessage.message;
    }

    return null;
  }

  private resolveFilters(message: any) {
    var filtersToRemove: ICallback[] = [];

    this.waitFilters.forEach(x => {
      if (x.filter(message)) {
        filtersToRemove.push(x);
        x.resolve(message);
      }
    });

    filtersToRemove.forEach(x => {
      this.waitFilters = this.waitFilters.filter(y => x !== y);
    });
  }
}
