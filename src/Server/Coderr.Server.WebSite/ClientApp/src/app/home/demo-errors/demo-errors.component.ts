import { Component, OnInit } from '@angular/core';
import * as Demo from "../../../server-api/Common/Demo";
import { ApiClient } from "../../utils/HttpClient";

export interface IDemoItem {
  description: string;
  title: string;
  id: string;
  selected: boolean;
}

interface IDemoCategory {
  name: string;
  items: IDemoItem[];
}

@Component({
  selector: 'app-demo-errors',
  templateUrl: './demo-errors.component.html',
  styleUrls: ['./demo-errors.component.scss']
})
export class DemoErrorsComponent implements OnInit {
  framework: string = '';
  demoIncidents: IDemoCategory[] = [];
  generated=false;

  constructor(private apiClient: ApiClient) { }

  ngOnInit(): void {
  }


  async showDemoOptions() {
    var dto = new Demo.GetDemoIncidentOptions();
    var result = await this.apiClient.query<Demo.GetDemoIncidentOptionsResult>(dto);
    this.demoIncidents = [];
    var category: IDemoCategory = null;

    result.items.forEach(dto => {
      if (this.framework === 'nodejs') {
        if (dto.category !== 'JavaScript' && dto.category !== "VueJS") {
          return;
        }
      }

      if (category == null || dto.category !== category.name) {
        category = {
          name: dto.category,
          items: []
        };

        this.demoIncidents.push(category);
      }

      var item = {
        id: dto.id,
        description: dto.description,
        title: dto.title,
        selected: false
      };
      category.items.push(item);
    });

  }

  generateErrors() {
    var itemsToGenerate: string[] = [];
    var libs: string[] = [];
    this.demoIncidents.forEach(category => {
      category.items.forEach(dto => {
        if (dto.selected) {
          itemsToGenerate.push(dto.id);
          if (!libs.find(x => x === category.name)) {
            libs.push(category.name);
          }
        }
      });
    });
    var cmd = new Demo.GenerateDemoIncidents();
    cmd.demoOptionIds = itemsToGenerate;
    this.apiClient.command(cmd);
    this.generated = true;
  }

  selectFramework(value: string) {
    this.framework = value;
    this.showDemoOptions();
  }

  toggleMe(item: IDemoItem) {
    item.selected = !item.selected;
  }


}
