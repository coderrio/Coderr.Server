import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateApiKeyComponent } from './create.component';

describe('CreateComponent', () => {
  let component: CreateApiKeyComponent;
  let fixture: ComponentFixture<CreateApiKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateApiKeyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateApiKeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
