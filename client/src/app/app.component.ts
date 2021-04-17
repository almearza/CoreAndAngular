import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  ngOnInit(): void {
    this.setCurrentUser();
  }
  setCurrentUser(){
    //if we close browser and open it agian we get this value from storage and set the observable
   const user:User=JSON.parse(localStorage.getItem('user'));
   this.accountService.setCurrentUser(user);
  }
  constructor(private accountService:AccountService) {
  }
 
}


