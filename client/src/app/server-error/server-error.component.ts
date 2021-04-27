import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { errorResponse } from '../_models/errorResponse';
@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})

export class ServerErrorComponent implements OnInit {
error:errorResponse;
  constructor(private router:Router) { 
    const navExtra=router.getCurrentNavigation();
    this.error=(navExtra?.extras?.state?.error) as errorResponse;
  }

  ngOnInit(): void {
  }

}
