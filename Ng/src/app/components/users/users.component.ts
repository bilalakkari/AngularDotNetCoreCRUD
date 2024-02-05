import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ApiService, USER } from 'src/app/services/api.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  formValue !: FormGroup;

  user: USER = new USER();
  allUsersData: any[] = [];

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
  ) { }

  ngOnInit(): void {
    this.formValue = this.fb.group({
      ID: [''],
      FNAME: [''],
      LNAME: [''],
      EMAIL: [''],
    })
    this.getAllUsers();
  }

  getAllUsers() {
    this.api.getAllUsers()
      .subscribe((res: any) => {
        this.allUsersData = res;
      })
  }

  addNewUser(form: any) {
    this.user.fname = form.value.FNAME;
    this.user.lname = form.value.LNAME;
    this.user.email = form.value.EMAIL;

    if (form.value.ID != null) {
      this.user.id = form.value.ID;
    } else {
      this.user.id = -1;
    }

    // console.log(this.user);

    this.api.addNewUser(this.user)
      .subscribe(
        (res: any) => {
          this.formValue.reset();
          this.getAllUsers();
        },
        (error: any) => {
          console.error('Error adding user:', error);
        }
      );
  }

  deleteUser(user: any) {
    const cfr = confirm("are you sure you want to delete this user ?")
    if (cfr == true) {
      this.api.deleteUser(user.id)
        .subscribe(
          (res: any) => {
            this.getAllUsers();
          });
    }
    else {
      return
    }
  }


  editUser(user: any) {
    console.log(user);
    this.formValue.setValue({
      ID: user.id,
      FNAME: user.fname,
      LNAME: user.lname,
      EMAIL: user.email,
    })
  }

}