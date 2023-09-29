
type MyHandler<T> = () => Promise<T>;


export class ExecuteOnce<T> {
  private _promise: Promise<T>;
  private acceptCallback: (value: T | PromiseLike<T>) => void;
  private rejectCallback: (reason?: any) => void;
  private executed = false;

  constructor(private callback: MyHandler<T>) {
    this._promise = new Promise<T>((accept, reject) => {
      this.acceptCallback = accept;
      this.rejectCallback = reject;
    });
  }

  get promise(): Promise<T> {
    return this._promise;
  }

  execute() {
    if (this.executed) {
      return;
    }
    this.executed = true;

    this.callback()
      .then(x => this.acceptCallback(x))
      .catch(x => this.rejectCallback(x));
  }
}

/**
 *
 */
export class PromiseWrapper<T> {
  private _promise: Promise<T>;
  private acceptCallback: (value: T | PromiseLike<T>) => void;
  private rejectCallback: (reason?: any) => void;
  private _completed: boolean;

  constructor() {
    this._promise = new Promise<T>((accept, reject) => {
      this.acceptCallback = accept;
      this.rejectCallback = reject;
    });
  }

  get promise(): Promise<T> {
    return this._promise;
  }

  get completed(): boolean {
    return this._completed;
  }

  accept(value: T) {
    this._completed = true;
    this.acceptCallback(value);
  }

  reject(error: Error) {
    this._completed = true;
    this.rejectCallback(error);
  }

}
