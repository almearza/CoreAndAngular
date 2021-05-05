import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { url } from 'node:inspector';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
@Input() member:Member;

uploader:FileUploader;
hasBaseDropZoneOver=false;
baseUrl=environment.baseUrl;
user:User;
  constructor(private accountService:AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
      this.user=user;
    })
  }
  fileOverBase(e:any){
  this.hasBaseDropZoneOver=e;
}
  ngOnInit(): void {
  }
initializeUploader(){
  this.uploader=new FileUploader({
    url:this.baseUrl+'users/add-photo',
    authToken:this.user.token,
    allowedFileType:['image'],
    removeAfterUpload:true,
    autoUpload:false,
    isHTML5:true,
    maxFileSize:10*1024*1024
  });
  this.uploader.onAfterAddingFile=(file)=>{
    file.withCredentials=false;
  }
  this.uploader.onSuccessItem=(item,response,status,headers)=>{
    if(response){
      const photo = JSON.parse(response);
      this.member.photos.push(photo);
    }
  }
}
}
