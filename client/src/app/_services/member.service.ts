import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
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
  members: Member[]=[];
  private _baseUrl = environment.baseUrl;
  public get baseUrl() {
    return this._baseUrl;
  }
  public set baseUrl(value) {
    this._baseUrl = value;
  }
  constructor(private http: HttpClient) { }
  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }
  getMember(username: string) {
    const member = this.members.find(m => m.username == username);
    if (member != undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);//,httpOptions
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
}
