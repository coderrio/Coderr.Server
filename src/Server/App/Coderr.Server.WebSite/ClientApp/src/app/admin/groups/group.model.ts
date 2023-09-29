export class Group implements IGroup {
  constructor(public id: number, public name: string) {
  }

  teams: IGroupTeam[] = [];
  applications: number[] = [];
}

export interface IGroup {
  readonly id: number;
  readonly name: string;
  readonly teams: IGroupTeam[];
  readonly applications: number[];
}

export interface IGroupListItem {
  readonly id: number;
  readonly name: string;
}


export interface IGroupTeam {
  readonly teamId: number;
  readonly teamName: string;
}
