import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = "https://localhost:5001/api/";
  arrayOfErrors:string[]=[];
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }
  get404Error() {
    return this.http.get(this.baseUrl+"buggy/not-found").subscribe(response=>{
      console.log(response);
    },error=>{
      console.log(error);
      
    })
  }
  get401Error() {
    return this.http.get(this.baseUrl+"buggy/auth").subscribe(response=>{
      console.log(response);
    },error=>{
      console.log(error);
      
    })
  }
  get500Error() {
    return this.http.get(this.baseUrl+"buggy/server-error").subscribe(response=>{
      console.log(response);
    },error=>{
      console.log(error);
      
    })
  }
  get400Error() {
    return this.http.post(this.baseUrl+"account/register",{}).subscribe(response=>{
      console.log(response);
    },error=>{
      console.log(error);
      // error.forEach(error => {
      //   this.arrayOfErrors.push(error);
      // });
      this.arrayOfErrors=error
    })
  }
}
