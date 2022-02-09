interface IHubEntity {
  Namespace: string;
  TypeName: string;
  IsEvent: boolean;
}

interface IHubEvent {
  typeName: string;
  body: any;
  correlationId: string;
}
