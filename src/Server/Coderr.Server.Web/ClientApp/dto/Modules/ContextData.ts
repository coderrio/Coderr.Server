export class GetSimilarities
{
    public static TYPE_NAME: string = 'GetSimilarities';
    public IncidentId: number;
}
export class GetSimilaritiesCollection
{
    public static TYPE_NAME: string = 'GetSimilaritiesCollection';
    public Name: string;
    public Similarities: GetSimilaritiesSimilarity[];
}
export class GetSimilaritiesResult
{
    public static TYPE_NAME: string = 'GetSimilaritiesResult';
    public Collections: GetSimilaritiesCollection[];
}
export class GetSimilaritiesSimilarity
{
    public static TYPE_NAME: string = 'GetSimilaritiesSimilarity';
    public Name: string;
    public Values: GetSimilaritiesValue[];
}
export class GetSimilaritiesValue
{
    public static TYPE_NAME: string = 'GetSimilaritiesValue';
    public Count: number;
    public Percentage: number;
    public Value: string;
}
