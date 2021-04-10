import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  ngOnInit(): void {
    this.getClient();
  }
  users: any;
  title = 'client';
  constructor(private httpClient: HttpClient) {
  }
  getClient() {
    this.httpClient.get("https://localhost:5001/api/users").subscribe(response => {
      this.users = response;
    },error=>{console.log(error);
     });
  }
}


