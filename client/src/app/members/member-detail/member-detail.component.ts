import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private memberService: MemberService, private router: ActivatedRoute) { }

  ngOnInit(): void {
    this.getMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }
    ];
    
  }
  getImages():NgxGalleryImage[]{
    const ngxImages=[];
      for(const photo of this.member.photos){
        ngxImages.push({
          small:photo?.url,
          medium:photo?.url,
          big:photo?.url
        });
      }
    return ngxImages;
  }
  getMember() {
    this.memberService.getMember(this.router.snapshot.paramMap.get('username'))
      .subscribe(member => {
        this.member=member;
        this.galleryImages=this.getImages();
      });
  }
}
