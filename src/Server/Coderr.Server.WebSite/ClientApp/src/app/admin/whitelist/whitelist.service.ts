import { Injectable } from '@angular/core';
import { ApiClient } from "../../utils/HttpClient";
import * as whitelist from "../../../server-api/Core/Whitelist";
import { WhitelistEntry, IWhitelistApp, IIpAddress, IpType } from "./whitelist.model";
import { ApplicationService } from "../../applications/application.service";

@Injectable({
  providedIn: 'root'
})
export class WhitelistService {

  constructor(
    private apiClient: ApiClient,
    private applicationService: ApplicationService) { }

  async list(): Promise<WhitelistEntry[]> {
    var entities: WhitelistEntry[] = [];
    var q = new whitelist.GetWhitelistEntries();
    var result = await this.apiClient.query<whitelist.GetWhitelistEntriesResult>(q);
    result.entries.forEach(dto => {

      const entity = this.convertEntry(dto);
      entities.push(entity);
    });

    return entities;
  }

  async get(id: number): Promise<WhitelistEntry> {
    if (!id) {
      throw new Error("Must specify an id.");
    }

    var q = new whitelist.GetWhitelistEntries();
    var result = await this.apiClient.query<whitelist.GetWhitelistEntriesResult>(q);

    const item = result.entries.find(x => x.id === id);
    if (!item) {
      throw new Error("Failed to find item " + id);
    }

    return this.convertEntry(item);
  }

  async add(entry: WhitelistEntry): Promise<void> {
    if (!entry) {
      throw new Error("Must specify an entry to add.");
    }

    var cmd = new whitelist.AddEntry();
    cmd.applicationIds = entry.applications.map(x=>x.id);
    cmd.domainName = entry.domainName;
    cmd.ipAddresses = entry.ipAddresses.map(x => x.address);
    this.apiClient.command(cmd);
  }

  async update(entry: WhitelistEntry): Promise<void> {
    if (!entry) {
      throw new Error("Must specify an entry to update it.");
    }

    if (!entry.id) {
      throw new Error("Must specify an entry ID to update it.");
    }

    var cmd = new whitelist.EditEntry();
    cmd.id = entry.id;
    cmd.applicationIds = entry.applications.map(x => x.id);
    cmd.domainName = entry.domainName;
    cmd.ipAddresses = entry.ipAddresses.map(x => x.address);
    this.apiClient.command(cmd);
  }

  async remove(id: number) {
    if (!id) {
      throw new Error("Must specify an ID to remove the entry.");
    }

    var cmd = new whitelist.RemoveEntry();
    cmd.id = id;
    this.apiClient.command(cmd);
  }

  private convertEntry(dto: whitelist.GetWhitelistEntriesResultItem): WhitelistEntry {
    if (!dto) {
      throw new Error("Must specify a DTO for conversion");
    }

    var entity = new WhitelistEntry(dto.domainName, dto.id);
    entity.applications = dto.applications.map(x => {
      return {
        id: x.applicationId,
        name: x.name,
        selected: true
      };
    });

    entity.ipAddresses = dto.ipAddresses.map(x => {
      return {
        id: x.id,
        address: x.address,
        type: <number>x.type
      };
    });

    return entity;
  }
}
