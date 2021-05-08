import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // model: any = {};
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate:Date;
  validationErrors:string[]=[];
  constructor(private accountService: AccountService, 
    private toastr: ToastrService,private fb:FormBuilder,private router:Router) { }

  ngOnInit(): void {
    this.iniailizeForm();
    this.maxDate=new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
  iniailizeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownUs: ['', Validators.required],
      birthDate: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required,this.matchValue("password")]],

    });
    this.registerForm.controls.password.valueChanges.subscribe(()=>{
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }
  matchValue(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      //null means no errors
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true }
    }
  }
  register() {
    this.accountService.register(this.registerForm.value).subscribe(user=>{
      this.router.navigateByUrl('/members');
    },error=>{
      this.validationErrors=error;
    });
    // this.cancel();
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
