export class CreatePartition {
    public static TYPE_NAME: string = 'CreatePartition';
    ApplicationId: number;
    Name: string;
    PartitionKey: string;
    NumberOfItems: number;
    Weight: number;
}
export class UpdatePartition {
    public static TYPE_NAME: string = 'UpdatePartition';
    Id: number;
    Name: string;
    NumberOfItems: number;
    Weight: number;
}
export class DeletePartition {
    public static TYPE_NAME: string = 'DeletePartition';
    Id: number;
}

export class GetPartitions {
    public static TYPE_NAME: string = 'GetPartitions';
    ApplicationId: number;
}

export class GetPartitionsResult {
    Items: GetPartitionsResultItem[];
}

export class GetPartitionsResultItem {
    Id: number;
    ApplicationId: number;
    Name: string;
    PartitionKey: string;
    NumberOfItems: number;
    Weight: number;
}

export class GetPartition {
    public static TYPE_NAME: string = 'GetPartition';
    Id: number;
}

export class GetPartitionResult {
    Id: number;
    ApplicationId: number;
    Name: string;
    PartitionKey: string;
    NumberOfItems: number;
    Weight: number;
}
