import { Injectable } from '@angular/core';
import * as api from "../../../../server-api/Common/Partitions";
import { ApiClient } from "../../../utils/HttpClient";
import "../../../validation";
import { required, stringLength, range2, copy, validate, range } from "../../../validation";

export interface IPartitionListItem {
  id: number;
  applicationId: number;
  name: string;
  partitionKey: string;
  weight: number;
}

export class Partition {
  id = 0;

  @required
  @range(1)
  applicationId: number = 0;

  @required
  @stringLength(40)
  name: string = '';

  @required
  @stringLength(20)
  partitionKey: string = '';

  numberOfItems?: number = null;
  weight: number = 1;
  importantThreshold?: number = null;
  criticalThreshold?: number = null;
}

@Injectable({
  providedIn: 'root'
})
export class PartitionService {
  private listPromises = new Map<number, Promise<IPartitionListItem[]>>();
  private partitionPromises = new Map<number, Promise<Partition>>();

  constructor(private apiClient: ApiClient) {

  }


  public async listForApplication(applicationId: number): Promise<IPartitionListItem[]> {
    var promise = this.listPromises.get(applicationId);
    if (promise != null) {
      return await promise;
    }

    var accept: any;
    var reject: any;
    this.listPromises[applicationId] = new Promise((acceptInner, rejectInner) => {
      accept = acceptInner;
      reject = rejectInner;
    });

    var query = new api.GetPartitions();
    query.applicationId = applicationId;
    try {
      var result = await this.apiClient.query<api.GetPartitionsResult>(query);

      var partitions: IPartitionListItem[] = result.items;
      accept(partitions);
      return partitions;
    } catch (error) {
      reject(error);
      throw error;
    }
  }

  public async get(id: number): Promise<Partition> {
    var promise = this.partitionPromises.get(id);
    if (promise != null) {
      return await promise;
    }

    var accept: any;
    var reject: any;
    this.partitionPromises[id] = new Promise((acceptInner, rejectInner) => {
      accept = acceptInner;
      reject = rejectInner;
    });

    var query = new api.GetPartition();
    query.id = id;
    try {
      var result = await this.apiClient.query<api.GetPartitionResult>(query);
      console.log('loaded part', result);
      var partitions: Partition = result;
      accept(partitions);
      return partitions;
    } catch (error) {
      reject(error);
      throw error;
    }
  }

  public async create(partition: Partition): Promise<void> {
    if (partition == null) {
      throw new Error("Partition must be specified.");
    }
    var errors = validate(partition);
    if (errors.length > 0) {
      throw new Error(errors.join(','));
    }

    var cmd = new api.CreatePartition();
    copy(partition, cmd);
    await this.apiClient.command(cmd);
  }

  public async update(partition: Partition): Promise<void> {
    if (partition == null) {
      throw new Error("Partition must be specified.");
    }

    console.log('partu', partition);
    var errors = validate(partition);
    if (errors.length > 0) {
      throw new Error(errors.join(','));
    }
    console.log('partu2', partition);

    var cmd = new api.UpdatePartition();
    copy(partition, cmd, {skipExistenceCheck: true});
    await this.apiClient.command(cmd);
  }

}
