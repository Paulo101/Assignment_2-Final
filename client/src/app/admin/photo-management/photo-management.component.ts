import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  @Input photo: Photo;
  photos : Photo[] = [];
  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
  }
  getPhotoForApproval()
  {
    this.adminService.getPhotosForApproval().subscribe(() => {
    });
  }
  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
     this.adminService.photo =  this.photo;
    });
  }
  rejectPhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
     this.adminService.photo =  this.photo;
    });
  }

}
