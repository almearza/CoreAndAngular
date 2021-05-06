import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
//injectable
//singleton:it survive while the app is running
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl=environment.baseUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$=this.currentUserSource.asObservable();
  constructor(private http:HttpClient) { }
  login(model:User){
    return this.http.post(this.baseUrl+"account/login",model).pipe(map((response:any)=>{
      const user=response;
      if(user){
        this.setCurrentUser(user);
      }
      return user;
      
    }));
  }
  register(user:any){
    return this.http.post(this.baseUrl+"account/register",user).pipe(map((user:User)=>{
      this.setCurrentUser(user);
    }));
  }
  setCurrentUser(user:User){
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
