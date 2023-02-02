import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnChanges, OnInit } from '@angular/core';

@Component({
  selector: 'app-employees',
  templateUrl: './employees.component.html'
})
export class EmployeesComponent {
  public employees: Employee[] = [];
  private url!: string;

  longestWorkingPair: [number, number, number] = [-1, -1, -1];

  constructor(
    private http: HttpClient, 
    @Inject('BASE_URL') baseUrl: string) 
  { 
    this.url = baseUrl;
  }

  onImportEmployees() {
    this.http
    .get(this.url + 'employees/ImportEmployees')
    .subscribe(result => {
      if(result) {
        alert("Employees imported!");
        console.log(result);
      } else { 
        alert("Employees could not be imported!");
      }
    });
  }

  onUploadFile(event: any) {
    const file = event.target.files[0];
    const formData = new FormData();
    formData.append('file', file);

    this.http.post<{item1: number, item2: number, item3: number}>('employees/ImportFile', formData)
    .subscribe(response => {
      this.longestWorkingPair[0] = response.item1;
      this.longestWorkingPair[1] = response.item2;
      this.longestWorkingPair[2] = response.item3;
    })
  }
}

interface Employee {
  id: number;
  projectId: number;
  dateFrom: string;
  dateTo: string;
}
