import { Component, OnInit } from '@angular/core';
import { likesParams } from 'src/app/_models/likesParams';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]> = [];
  likeParams: likesParams = new likesParams();
  pagination:Pagination;  

  constructor(private memberService:MembersService) { 
    this.likeParams.predicate = 'liked'; 
  }

  ngOnInit(): void { 
    this.loadLikes();
  }

  loadLikes(){    
    return this.memberService.getLikes(this.likeParams).subscribe((response) => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  pageChanged(event: any){
    this.likeParams.pageNumber = event.page;
    this.loadLikes();
  }
}
