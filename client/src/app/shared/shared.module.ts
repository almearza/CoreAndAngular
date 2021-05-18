import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    TabsModule,
    ModalModule.forRoot(),
  ],
  exports:[
    TabsModule,
    ModalModule
  ]
})
export class SharedModule { 
  
}
