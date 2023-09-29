import { Component, OnInit, Input } from '@angular/core';
import { ModalService } from "../../../_controls/modal/modal.service";
import { ToastrService } from "ngx-toastr";
import { AccountService, User } from "../../../accounts/account.service";

@Component({
  selector: 'account-settings-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  constructor(
    private modalService: ModalService,
    private toastrService: ToastrService,
    private accountService: AccountService
  ) {
  }

  ngOnInit(): void {
  }


}
