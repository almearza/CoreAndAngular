import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/Photo';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { MemberService } from '../_services/member.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.baseUrl;
  user: User;
  constructor(private accountService: AccountService, private memberService: MemberService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }
  ngOnInit(): void {
    this.initializeUploader();
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      isHTML5: true,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);

      }
    }
  }
  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain = false;
        if (p.id == photo.id) p.isMain = true;
      });
    })
  }
  deletePhoto(photo:Photo){
    this.memberService.deletePhoto(photo.id).subscribe(()=>{
      // const index =this.member.photos.indexOf(photo);
      // this.member.photos.splice(index,1);
      this.member.photos= this.member.photos.filter(p=>p.id!=photo.id);
    })
  }
}
