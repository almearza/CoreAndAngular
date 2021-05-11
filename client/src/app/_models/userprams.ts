import { User } from "./user";

export class UserPrams{
    gender:string;
    pageNumber=1;
    pageSize=5;
    minAge=18;
    maxAge=99;
    orderBy='lastActive';

    constructor(user:User) {
        this.gender=user.gender;
    }
}