import { HttpClient, HttpHeaders, HttpParams, HttpProgressEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Paginatted } from '../_models/pagination';
import { User } from '../_models/user';
import { UserPrams } from '../_models/userprams';
import { AccountService } from './account.service';
// const httpOptions={
//   headers:new HttpHeaders({
//     Authorization:"Bearer "+JSON.parse(localStorage.getItem('user'))?.token
//   })
// }
@Injectable({
  providedIn: 'root'
})
export class MemberService {
  // members: Member[] = [];
  user: User;
  userPrams: UserPrams;
  memberCach = new Map();
  private _baseUrl = environment.baseUrl;
  public get baseUrl() {
    return this._baseUrl;
  }
  public set baseUrl(value) {
    this._baseUrl = value;
  }
  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    });
    this.userPrams = new UserPrams(this.user);
  }
  getUserPrams() {
    return this.userPrams;
  }
  setUserPrams(prams: UserPrams) {
    this.userPrams = prams;
  }
  resetFilters(){
    this.userPrams=new UserPrams(this.user);
    return this.userPrams;
  }
  getMembers(userPrams: UserPrams) {
    let membersFromCach = this.memberCach.get(Object.values(userPrams).join('_'));
    if (membersFromCach)
      return of(membersFromCach);
    let prams = this.getPrams(userPrams);
    return this.getPaginattedResult<Member[]>(this.baseUrl + 'users', prams)
      .pipe(map(response => {
        this.memberCach.set(Object.values(userPrams).join('_'), response);
        return response;
      }));
  }
  getMember(username: string) {
    // const member = this.members.find(m => m.username == username);
    // if (member != undefined) return of(member);
    const member = [...this.memberCach.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);//,httpOptions
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
    // .pipe(
    //   map(() => {
    //     const index = this.members.indexOf(member);
    //     this.members[index] = member;
    //   })
    // );
  }
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
  getPrams(userPrams: UserPrams): HttpParams {
    let httpPrams = new HttpParams();
    httpPrams = httpPrams.append('pageNumber', userPrams.pageNumber.toString());
    httpPrams = httpPrams.append('pageSize', userPrams.pageSize.toString());
    httpPrams = httpPrams.append('minAge', userPrams.minAge.toString());
    httpPrams = httpPrams.append('maxAge', userPrams.maxAge.toString());
    httpPrams = httpPrams.append('gender', userPrams.gender);
    httpPrams = httpPrams.append('orderBy', userPrams.orderBy);
    return httpPrams;
  }
  getPaginattedResult<T>(url: string, httpPrams: HttpParams) {
    let paginattedResult: Paginatted<T> = new Paginatted();
    return this.http.get<T>(url, { observe: 'response', params: httpPrams }).pipe(
      map(response => {
        paginattedResult.result = response.body;
        let paginationHeader = response.headers.get('Pagination');
        if (paginationHeader != null) {
          paginattedResult.pagination = JSON.parse(paginationHeader);
        }
        return paginattedResult;
      })
    );
  }
}

