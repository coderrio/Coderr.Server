import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../../../application.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NavMenuService } from "../../../../nav-menu/nav-menu.service";
import { PartitionService } from "../partition.service";

@Component({
  selector: 'partition-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.scss']
})
export class PartitionNewComponent implements OnInit, OnDestroy {
  weight = 0;
  name = '';
  partitionKey = '';

  numberOfItems?= null;
  importantThreshold?= null;
  criticalThreshold?= null;

  private id = 0;
  private sub: any;

  constructor(private appService: ApplicationService,
    private partitionService: PartitionService,
    private route: ActivatedRoute,
    private menuService: NavMenuService,
    private router: Router) { }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['applicationId'];
      this.appService.get(this.id).then(x => {

        this.menuService.updateNav([
          { title: x.name, route: ['application', x.id] },
          { title: "Administration", route: ['application', x.id, 'admin'] },
          { title: "New partition", route: ['application', x.id, 'admin', 'partitions/new'] }
        ]
        );

      });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  async save(): Promise<void> {
    await this.partitionService.create({
      name: this.name,
      partitionKey: this.partitionKey,
      numberOfItems: this.numberOfItems,
      importantThreshold: this.importantThreshold,
      criticalThreshold: this.criticalThreshold,
      applicationId: this.id,
      weight: this.weight,
      id: 0
    });
    this.router.navigate(['/application', this.id, 'admin']);

  }

  cancel() {
    this.router.navigate(['/application', this.id, 'admin']);
  }
}
