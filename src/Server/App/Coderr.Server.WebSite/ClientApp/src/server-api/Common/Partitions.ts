
export class CreatePartition {
  public static TYPE_NAME: string = 'CreatePartition';
  applicationId: number;
  name: string;
  partitionKey: string;
  numberOfItems: number;
  weight: number;
  importantThreshold?: number;
  criticalThreshold?: number;
}
export class UpdatePartition {
  public static TYPE_NAME: string = 'UpdatePartition';
  id: number;
  name: string;
  numberOfItems: number;
  weight: number;
  importantThreshold?: number;
  criticalThreshold?: number;
}
export class DeletePartition {
  public static TYPE_NAME: string = 'DeletePartition';
  id: number;
}

export class GetPartitions {
  public static TYPE_NAME: string = 'GetPartitions';
  applicationId: number;
}

export class GetPartitionsResult {
  items: GetPartitionsResultItem[];
}

export class GetPartitionsResultItem {
  id: number;
  applicationId: number;
  name: string;
  partitionKey: string;
  weight: number;
}

export class GetPartition {
  public static TYPE_NAME: string = 'GetPartition';
  id: number;
}

export class GetPartitionResult {
  id: number;
  applicationId: number;
  name: string;
  partitionKey: string;
  numberOfItems: number;
  weight: number;
  importantThreshold?: number;
  criticalThreshold?: number;
}

export class GetPartitionValues {
  public static TYPE_NAME: string = 'GetPartitionValues';
  partitionId: number;
  incidentId?: number;
  pageNumber?: number;
  pageSize: number = 20;
}

export class GetPartitionValuesResult {
  items: GetPartitionValuesResultItem[];
}

export class GetPartitionValuesResultItem {
  id: number;
  value: number;
  receivedAtUtc: Date;
}

export class GetPartitionInsights {
  public static TYPE_NAME: string = 'GetPartitionInsights';
  incidentId?: number;
  applicationIds: number[];
  startDate: Date;
  endDate: Date;
  summarizePeriodStartDate: Date;
  summarizePeriodEndDate: Date;
}

export class GetPartitionInsightsResult {
  applications: GetPartitionInsightsResultApplication[];
}

export class GetPartitionInsightsResultApplication {
  applicationId: number;
  indicators: GetPartitionInsightsResultIndicator[];
}

export class GetPartitionInsightsResultIndicator {
  name: string;
  displayName: string;
  value: number;
  periodValue: number;
  dates: string[];
  values: string[];
}
