import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
// const httpOptions={
//   headers:new HttpHeaders({
//     Authorization:"Bearer "+JSON.parse(localStorage.getItem('user'))?.token
//   })
// }
@Injectable({
  providedIn: 'root'
})
export class MemberService {
baseUrl=environment.baseUrl;
  constructor(private http:HttpClient) { }
  getMembers(){
    return this.http.get<Member[]>(this.baseUrl+'users');
  }
  getMember(username:string){
    return this.http.get<Member>(this.baseUrl+'users/'+username);//,httpOptions
  }
}
