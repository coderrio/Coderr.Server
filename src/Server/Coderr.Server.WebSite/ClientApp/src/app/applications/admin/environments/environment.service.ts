import { Injectable } from '@angular/core';
import * as api from "../../../../server-api/Core/Environments";
import { ApiClient } from "../../../utils/HttpClient";
import { required, copy, validate } from "../../../validation";

export class Environment {
  constructor(applicationId: number, name: string) {
    this.applicationId = applicationId;
    this.name = name;
  }

  id?: number = null;

  @required
  applicationId: number;


  @required
  name: string = "";

  @required
  ignoreErrorReports: boolean = false;
}


@Injectable({
  providedIn: 'root'
})
export class EnvironmentService {
  private environments = new Map<number, Environment[]>();
  private loadPromises = new Map<number, Promise<Environment[]>>();

  constructor(private apiClient: ApiClient) {

  }

  async list(applicationId: number): Promise<Environment[]> {
    var promise = this.loadPromises.get(applicationId);
    if (promise) {
      return await promise;
    }

    promise = this.listInner(applicationId);
    this.loadPromises.set(applicationId, promise);

    this.environments.set(applicationId, await promise);
    return this.environments.get(applicationId);
  }

  async get(applicationId: number, id: number) {
    var envs = await this.list(applicationId);

    var entry = envs.find(x => x.id === id);
    if (!entry) {
      throw new Error("Entry with id " + id + "was not found");
    }

    return entry;
  }

  async create(env: Environment) {
    if (!env) {
      throw new Error("Environment must be specified.");
    }

    var errors = validate(env);
    if (errors.length > 0) {
      throw new Error(errors.join(', '));
    }

    var cmd = new api.CreateEnvironment();
    copy(env, cmd);
    await this.apiClient.command(cmd);

    var query = new api.GetEnvironments();
    query.applicationId = env.applicationId;
    var dtos = await this.apiClient.query<api.GetEnvironmentsResult>(query);

    var dto = dtos.items.find(x => x.name === env.name);
    env.id = dto.id;

    var envs = await this.list(env.applicationId);
    envs.push(env);
  }

  async update(updated: Environment) {
    var envs = await this.list(updated.applicationId);
    var env = envs.find(x => x.name === updated.name);

    env.ignoreErrorReports = updated.ignoreErrorReports;

    var cmd = new api.UpdateEnvironment();
    cmd.applicationId = updated.applicationId;
    cmd.deleteIncidents = updated.ignoreErrorReports;
    cmd.environmentId = updated.id;
    await this.apiClient.command(cmd);
  }

  async reset(applicationId: number, environmentId: number) {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified");
    }

    if (!environmentId) {
      throw new Error("EnvironmentId must be specified");
    }

    var cmd = new api.ResetEnvironment();
    cmd.applicationId = applicationId;
    cmd.environmentId = environmentId;
    await this.apiClient.command(cmd);
  }

  private async listInner(applicationId: number): Promise<Environment[]> {
    var query = new api.GetEnvironments();
    query.applicationId = applicationId;
    var envs = await this.apiClient.query<api.GetEnvironmentsResult>(query);
    return envs.items.map(x => {
      var e = new Environment(applicationId, x.name);
      e.id = x.id;
      e.ignoreErrorReports = x.deleteIncidents;
      return e;
    });
  }
}
