import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from '../_models/message';
import { MemberService } from '../_services/member.service';
import { MessageService } from '../_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('sendMessageForm') messageForm:NgForm;
  // @Input() messages: Message[] = [];
  @Input() username:string;
  messageContent:string;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {

  }
  sendMessage(){
    this.messageService.sendMessage(this.username,this.messageContent).then(()=>{
      // this.messages.push(message); this will be done by the hub
      this.messageForm.reset();
    })
  }

}
