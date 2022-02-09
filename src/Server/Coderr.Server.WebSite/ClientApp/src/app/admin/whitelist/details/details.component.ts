import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";
import { WhitelistService } from "../whitelist.service";
import { WhitelistEntry } from "../whitelist.model";
import { ModalService } from "../../../_controls/modal/modal.service";

@Component({
  selector: 'whitelist-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit, OnDestroy {
  whitelist: WhitelistEntry = new WhitelistEntry('temp.com');
  id: number;
  private routeSub: any;

  constructor(
    private service: WhitelistService,
    private modalService: ModalService,
    private route: ActivatedRoute,
    private router: Router,
    private navMenuService: NavMenuService,
  ) {


  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.load(this.id);
    });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }

  verifyRemove() {
    this.modalService.open('verifyRemoveModal');
  }

  async removeEntry(): Promise<void> {
    await this.service.remove(this.id);
    this.modalService.close('verifyRemoveModal');
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  cancelRemove() {
    this.modalService.close('verifyRemoveModal');
  }
  

  private async load(id: number): Promise<void> {
    this.whitelist = await this.service.get(id);

    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Whitelists', route: ['admin/whitelists'] },
      { title: this.whitelist.domainName, route: ['admin/whitelists/', id] }
    ]);

  }



}
