import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as api from "../../server-api/Core/Incidents";
import { ApiClient } from "../utils/HttpClient";

export interface INavPill {
  route: any[];
  title: string;
  id?: string;
}

@Injectable({
  providedIn: 'root'
})
export class NavMenuService {
  selectedApplicationId: BehaviorSubject<number>;
  showAsOnboarding: boolean = false;
  navItems: BehaviorSubject<INavPill[]>;
  hasIncidents = false;

  constructor(client: ApiClient) {
    this.selectedApplicationId = new BehaviorSubject<number>(0);
    this.navItems = new BehaviorSubject<INavPill[]>([]);
    var query = new api.FindIncidents();
    query.itemsPerPage = 1;
    query.pageNumber = 1;
    client.query<api.FindIncidentsResult>(query)
      .then(result => {
        this.hasIncidents = result.totalCount > 0;
      });

  }

  selectApplication(applicationId: number) {
    if (!applicationId || applicationId < 0)
      throw new Error("Must supply an application id");

    this.selectedApplicationId.next(applicationId);
  }

  updateNav(navItems: INavPill[]) {
    this.navItems.next(navItems);
  }

  selectGroup(id: number) {

  }
  
}
