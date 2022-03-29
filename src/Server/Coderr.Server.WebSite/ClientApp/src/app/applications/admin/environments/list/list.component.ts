import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EnvironmentService, Environment } from "../environment.service";
import { ApplicationService } from "../../../application.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'env-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class EnvironmentListComponent implements OnInit, OnDestroy {
  private sub: any;
  private applicationId: number;
  environments: Environment[] =  [];

  constructor(private service: EnvironmentService,
    private appService: ApplicationService,
    private route: ActivatedRoute,
    private noticeService: ToastrService) { }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.load();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  deleteErrors(env: Environment) {
    this.service.reset(env.id, env.applicationId);
    this.noticeService.success("Environment has been queued to be reset.");
  }

  private async load(): Promise<void> {
    this.environments = await this.service.list(this.applicationId);
  }

}
