import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-managment',
  templateUrl: './user-managment.component.html',
  styleUrls: ['./user-managment.component.css']
})
export class UserManagmentComponent implements OnInit {
  users: Partial<User[]> = [];
  bsModalRef: BsModalRef;
  constructor(private adminService: AdminService, private modalService: BsModalService) { }
  ngOnInit(): void {
    this.getUsers();
  }
  getUsers() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    });
  }
  openRolesModal(user: User) {
    const config = {
      class: "modal-dailog-centered",
      initialState: {
        user,
        roles:this.getUserRoles(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);

    this.bsModalRef.content.updateSelectedRoles.subscribe(values=>{
      const rolesToUpdate={
        roles:[...values.filter(el=>el.checked===true).map(el=>el.name)]
      };
      this.adminService.updateRoles(user.username,rolesToUpdate.roles).subscribe(()=>{
        user.roles=[...rolesToUpdate.roles];
      })
    });
  }
  private getUserRoles(user: User) {
    const roles=[];
    let avialableRoles: any[] = [
      { name: 'Admin', value: 'admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' }
    ];
    avialableRoles.forEach(role => {
      let isMatch=false;
      for (const userRole of user.roles) {
        if(role.name===userRole){
          isMatch=true;
          role.checked=true;
          roles.push(role);
          break;
        }
      }
      if(!isMatch){
        role.checked=false;
        roles.push(role);
      }
    });
    return roles;
  }
}
