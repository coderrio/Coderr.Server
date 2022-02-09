import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../../../application.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NavMenuService } from "../../../../nav-menu/nav-menu.service";
import { PartitionService, Partition } from "../partition.service";
import { copy } from "../../../../validation";

@Component({
  selector: 'partition-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class PartitionEditComponent implements OnInit {
  private applicationId = 0;
  private id = 0;
  private sub: any;
  private backendPartition: Partition;
  partition: Partition = new Partition();

  constructor(private appService: ApplicationService,
    private partitionService: PartitionService,
    private route: ActivatedRoute,
    private menuService: NavMenuService,
    private router: Router) { }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.id = +params['id'];
      this.load();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  async save(): Promise<void> {
    console.log('backendPart', this.backendPartition);
    copy(this.partition,
      this.backendPartition,
      {
        stringFields: ['name', 'partitionKey'],
        numericFields: ['weight', 'numberOfItems', 'importantThreshold', 'criticalThreshold'],
        skipExistenceCheck: true
      });
    console.log('backendPart2', this.backendPartition);

    await this.partitionService.update(this.backendPartition);

    this.router.navigate(['/application', this.applicationId, 'admin']);

  }

  cancel() {
    this.router.navigate(['/application', this.applicationId, 'admin']);
  }

  private async load(): Promise<void> {
    var app = await this.appService.get(this.applicationId);
    this.backendPartition = await this.partitionService.get(this.id);
    copy(this.backendPartition, this.partition);


    this.menuService.updateNav([
      { title: app.name, route: ['application', app.id] },
      { title: "Administration", route: ['application', app.id, 'admin'] },
      { title: "Edit partition", route: ['application', app.id, 'admin', 'partitions', this.id, 'edit'] }
    ]
    );


  }
}
