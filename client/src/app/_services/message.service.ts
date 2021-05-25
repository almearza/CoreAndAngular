import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginattedHeaders, getPaginattedResult } from './pagination';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.baseUrl;

  hubConnection: HubConnection;
  hubUrl = environment.hubUrl;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messagesThread$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient, private busyService: BusyService) { }
  addMessage(message: Message) {
    return this.addMessage(message);
  }
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let prams = getPaginattedHeaders(pageNumber, pageSize);
    prams = prams.append('container', container);
    return getPaginattedResult<Message[]>(this.http, this.baseUrl + 'messages', prams);
  }
  // getMessageThread(username: string) {
  //   return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  // }
  async sendMessage(username: string, content: string) {
    // return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content });
    return this.hubConnection.invoke('SendMessage', { recipientUsername: username, content }).catch(error => {
      console.log("invoking sendMessage() " + error);
    });
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
  createHubConnection(user: User, otherUsername: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, { accessTokenFactory: () => user.token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => {
      console.log(error);
    })
      .finally(() => {
        this.busyService.idle();
      });

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    });
    //push new message into the queue
    this.hubConnection.on('NewMessage', message => {
      this.messagesThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]);
      })
    });
    //listen for group updated to mark message as read
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(c => c.username === otherUsername)) {
        this.messagesThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if (!message.messageRead) {
              message.messageRead = new Date(Date.now());
            }
          });
          this.messageThreadSource.next([...messages]);//update messages array with new one
        })
      }
    })
  }
  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => {
        console.log(error);
        this.messageThreadSource.next([]);
      })
  }
}
