export interface Message {
    id: number;
    senderPhotoUrl: string;
    senderId: number;
    senderUsername: string;
    recipientPhotoUrl: string;
    recipientId: number;
    recipientUsername: string;
    content: string;
    messageSent: Date;
    messageRead?: Date;
}