export class GetSimilarities {
  public static TYPE_NAME: string = 'GetSimilarities';
  public incidentId: number;
}
export class GetSimilaritiesCollection {
  public static TYPE_NAME: string = 'GetSimilaritiesCollection';
  public name: string;
  public similarities: GetSimilaritiesSimilarity[];
}
export class GetSimilaritiesResult {
  public static TYPE_NAME: string = 'GetSimilaritiesResult';
  public collections: GetSimilaritiesCollection[];
}
export class GetSimilaritiesSimilarity {
  public static TYPE_NAME: string = 'GetSimilaritiesSimilarity';
  public name: string;
  public values: GetSimilaritiesValue[];
}
export class GetSimilaritiesValue {
  public static TYPE_NAME: string = 'GetSimilaritiesValue';
  public count: number;
  public percentage: number;
  public value: string;
}
