import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  // messages: Message[] = [];
  user: User;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private memberService: MemberService,
    private activatedRouter: ActivatedRoute,
    private router:Router,
    private messageService: MessageService,
    private toastr: ToastrService,
    public presenceService: PresenceService,
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
        this.user=user;
        this.router.routeReuseStrategy.shouldReuseRoute=()=>false;
      });
      
     }
  

  ngOnInit(): void {
    this.activatedRouter.data.subscribe(data => {
      this.member = data.member;//member:key we used when we add resolver to route in approuting
    });
    this.activatedRouter.queryParams.subscribe(pram => {
      pram.tab ? this.activateTab(pram.tab) : this.activateTab(0);
    });
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    this.galleryImages = this.getImages();

  }
  addLike() {
    this.memberService.addLike(this.member.username).subscribe(response => {
      this.toastr.success('you liked ' + this.member.knownUs);
    });
  }
  getImages(): NgxGalleryImage[] {
    const ngxImages = [];
    for (const photo of this.member.photos) {
      ngxImages.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }
    return ngxImages;
  }

  activateTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'messages') {
      this.messageService.createHubConnection(this.user, this.member.username);
      
    }else{
      
      
      this.messageService.stopHubConnection();
    }

  }
  ngOnDestroy(): void {
    
    this.messageService.stopHubConnection();
  }
  // loadMessages() {
  //   this.messageService.getMessageThread(this.member.username).subscribe(response => {
  //     this.messages = response;
  //   })
  // }
}
