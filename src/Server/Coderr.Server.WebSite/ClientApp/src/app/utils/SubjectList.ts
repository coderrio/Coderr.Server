import { BehaviorSubject, Subject } from "rxjs";

export class BehaviorSubjectList<TEntity>
{
  private _items: TEntity[] = [];
  private _subject: BehaviorSubject<TEntity[]> = new BehaviorSubject<TEntity[]>([]);
  private _added: Subject<TEntity> = new Subject();
  private _removed: Subject<TEntity> = new Subject();

  constructor(private sortFunc: (a: TEntity, b: TEntity) => number, items?: TEntity[]) {
    if (items) {
      this._items = items.sort(sortFunc);
      this.subject.next(items);
    }
  }

  get current(): TEntity[] {
    return this._items;
  }

  add(item: TEntity) {
    this._items.push(item);
    this._items.sort(this.sortFunc);
    this._added.next(item);
    this._subject.next(this._items);
  }

  addAll(items: TEntity[]) {
    items.forEach(item => {
      this._items.push(item);
    });
    this._items.sort(this.sortFunc);
    this._subject.next(this._items);
  }

  clear() {
    this._items.forEach(x => {
      this._removed.next(x);
    });
    this._items = [];
    this._subject.next(this._items);
  }

  remove(item: TEntity): boolean {
    const index = this._items.indexOf(item, 0);
    if (index === -1) {
      return false;
    }

    this._items.splice(index, 1);
    this._removed.next(item);
    this._subject.next(this._items);
    return true;
  }

  get added(): Subject<TEntity> {
    return this._added;
  }
  get removed(): Subject<TEntity> {
    return this._removed;
  }

  get subject(): BehaviorSubject<TEntity[]> {
    return this._subject;
  }

  

  find(predicate: (search: TEntity) => boolean): TEntity {
    return this._items.find(predicate);
  }
}
