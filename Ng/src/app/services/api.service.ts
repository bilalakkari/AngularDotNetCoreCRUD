import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private readonly url = "https://localhost:7060/api/User"

  constructor(
    private api: HttpClient,
  ) { }

  getAllUsers() {
    return this.api.get(`${this.url}/GetAllUsers`)
  }

  addNewUser(data: any) {
    return this.api.post(`${this.url}/AddNewUser`, data)
  }

  deleteUser(id: any) {
    return this.api.delete(`${this.url}/DeleteUser/${id}`);
  }



}



export class USER {
  id?: number;
  fname: string = "string";
  lname: string = "string";
  email: string = "string";
}