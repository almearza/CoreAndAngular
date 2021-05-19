import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { getPaginattedHeaders, getPaginattedResult } from './pagination';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.baseUrl;

  hubConnection: HubConnection;
  hubUrl = environment.hubUrl;
  private messageThreadSource=new BehaviorSubject<string[]>([]);
  messages$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient) { }
  addMessage(message: Message) {
    return this.addMessage(message);
  }
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let prams = getPaginattedHeaders(pageNumber, pageSize);
    prams = prams.append('container', container);
    return getPaginattedResult<Message[]>(this.http, this.baseUrl + 'messages', prams);
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }
  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content });
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
  createHubConnection(user: User,otherUsername:string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message', { accessTokenFactory: () => user.token })
      .withAutomaticReconnect()
      .build();
      this.hubConnection.start().catch(error=>{
        console.log(error);
      });
      this.hubConnection.on('ReceiveMessageThread',messages=>{
        this.messageThreadSource.next(messages);
      });
     
  }
  stopHubConnection(){
    this.hubConnection.stop().catch(error=>{
      console.log(error);
    })
  }
}
