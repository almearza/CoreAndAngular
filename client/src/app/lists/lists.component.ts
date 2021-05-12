import { Component, OnInit } from '@angular/core';
import { LikeParams } from '../_models/likesparams';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MemberService } from '../_services/member.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members:Partial<Member[]>;//partial means optional
  likeParams:LikeParams;
  pagination:Pagination;
  constructor(private memberService:MemberService) { }

  ngOnInit(): void {
    this.likeParams=new LikeParams();
    this.loadLikes();
  }
  loadLikes(){
    this.memberService.getLikes(this.likeParams).subscribe(response=>{
      this.members=response.result;
      this.pagination=response.pagination;
    })
    }
    pageChanged(event){
      this.likeParams.pageNumber=event.page;
      this.loadLikes();
    }
}
