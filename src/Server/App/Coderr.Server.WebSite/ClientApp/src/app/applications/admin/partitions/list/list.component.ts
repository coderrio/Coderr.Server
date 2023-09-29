import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { PartitionService, IPartitionListItem } from "../partition.service";

@Component({
  selector: 'partition-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class PartitionListComponent implements OnInit, OnDestroy {
  applicationId: number;
  partitions: IPartitionListItem[] = [];

  private sub: any;

  constructor(private service: PartitionService,
    private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.loadEverything();
    });

  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  async loadEverything(): Promise<void> {
    this.partitions = await this.service.listForApplication(this.applicationId);
  }

}
