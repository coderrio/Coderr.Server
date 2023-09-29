import { Component, OnInit, Input } from '@angular/core';
import { ToastrService } from "ngx-toastr";
import { ModalService } from "../../_controls/modal/modal.service";
import { AccountService } from "../../accounts/account.service";

@Component({
  selector: 'admin-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class AdminNavbarComponent implements OnInit {

  constructor(
    private modalService: ModalService,
    private toastrService: ToastrService,
    private accountService: AccountService
  ) {
  }

  ngOnInit(): void {
  }

  

}
