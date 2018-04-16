export interface Message {
    type: string;
    body: any;
}

export interface MessageContext {
    topic: string;
    message: Message;
}

export type ListenerCallback = (msgContext: MessageContext) => void;

export class PubSubService {
    private topics: Topic[] = [];
    static Instance: PubSubService = new PubSubService();

    subscribe(topicName: string, callback: ListenerCallback) {
        let topic = this.findTopic(topicName);
        if (topic == null) {
            topic = new Topic(topicName);
            this.topics.push(topic);
        }

        (<Topic>topic).subscribe(callback);
    }

    unsubscribe(topicName: string, callback: ListenerCallback) {
        const topic = this.findTopic(topicName);
        if (topic == null) {
            throw new Error(`Topic ${name} is not registered`);
        }
        topic.unsubscribe(callback);
    }

    publish(topicName: string, message: Message|any) {
        if (!message.hasOwnProperty("type")) {
            message = {
                body: message,
                type: undefined
            };
        }

        let topic = this.findTopic(topicName);
        if (topic == null) {
            // typically when the publisher is created before the consumer
            topic = new Topic(topicName);
            this.topics.push(topic);
        }

        topic.publish(message);
    }

    private findTopic(name: string): Topic|null {
        for (let i = 0; i < this.topics.length; i++) {
            if (this.topics[i].name === name)
                return this.topics[i];
        }
        return null;
        //throw new Error("hllo");
    }
}

class Topic {
    private listeners: ListenerCallback[] = [];
    private cachedMessages: Message[] = [];

    constructor(public name: string) {

    }

    subscribe(callback: ListenerCallback) {
        this.listeners.push(callback);
        if (this.cachedMessages.length > 0) {
            this.cachedMessages.forEach(x => {
                this.publish(x);
            });
            this.cachedMessages.length = 0;
        }
    }

    publish(msg: Message) {
        var ctx: MessageContext = {
            topic: this.name,
            message: msg
        };
        if (this.listeners.length === 0) {
            this.cachedMessages.push(msg);
        }

        this.listeners.forEach(x => {
            x(ctx);
        });
    }

    unsubscribe(callback: ListenerCallback) {
        this.listeners = this.listeners.filter(x => x === callback);
    }
}
