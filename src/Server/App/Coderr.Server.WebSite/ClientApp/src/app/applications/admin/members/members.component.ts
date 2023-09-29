import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApplicationService } from "../../application.service";
import { IApplicationMember } from "../../application.model";

@Component({
  selector: 'app-members',
  templateUrl: './members.component.html',
  styleUrls: ['./members.component.scss']
})
export class MembersComponent implements OnInit {
  id: number;
  members: IApplicationMember[] = [];

  constructor(
    private service: ApplicationService,
    route: ActivatedRoute) {

    this.id = +route.snapshot.params.applicationId;
    this.service.getMembers(this.id).then(x => {
      this.members = x;
    });
  }

  ngOnInit(): void {
  }

}
