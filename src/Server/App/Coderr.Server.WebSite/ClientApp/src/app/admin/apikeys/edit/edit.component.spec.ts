import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditApiKeyComponent } from './edit.component';

describe('EditComponent', () => {
  let component: EditApiKeyComponent;
  let fixture: ComponentFixture<EditApiKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditApiKeyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditApiKeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
