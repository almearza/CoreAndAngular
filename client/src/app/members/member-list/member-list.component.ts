import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserPrams } from 'src/app/_models/userprams';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  user: User;
  filterList = [{ value: 'male', display: 'Male' }, { value: 'female', display: 'Female' }]

  userPrams = new UserPrams();
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;
  // members$:Observable<Member[]>;
  constructor(private memberService: MemberService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    });
    this.userPrams.gender = this.user.gender;
  }

  ngOnInit(): void {
    // this.members$=this.memberService.getMembers();
    this.loadMembers();
  }
  loadMembers() {

    this.memberService.getMembers(this.userPrams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }
  pageChanged(event: any) {
    this.userPrams.pageNumber = event.page;
    this.loadMembers();
  }
  resetFilter(){
    this.userPrams.gender=this.user.gender;
    this.loadMembers();
  }
}
