import { Injectable } from '@angular/core';
import { Observable, Subject } from "rxjs";
import { Router } from '@angular/router';
import { SettingsService } from "../../utils/settings.service";
import { ExecuteOnce } from "../../PromiseWrapper";
import { Guide } from "../../../Guide";
import { GuideComponent } from "./guide.component";

class GuideSettings {
  constructor() {
    this.shownGuides = [];
  }

  /** Guides that have already been shown */
  shownGuides: string[];

  hasShown(name: string): boolean {
    return this.shownGuides.find(x => x === name) != null;
  }
}

@Injectable({
  providedIn: 'root'
})
export class GuideService {
  private guideManager = new Guide();
  private settings = new GuideSettings();
  private loadHandler = new ExecuteOnce<GuideSettings>(() => this.load());
  private availableGuides: GuideComponent[] = [];
  private pageHaveGuides: Subject<boolean> = new Subject<boolean>();
  private activeGuide = "";
  private isLoaded=false;

  constructor(
    private settingsService: SettingsService,
    private router: Router) {

    if (!this.settingsService) {
      throw new Error("Service was not included");
    }

    this.loadHandler.execute();
    this.guideManager.closed = () => this.guideClosed();
    this.guideManager.showNext = () => this.showNextGuide();
  }

  get guidesAvailable(): Observable<boolean> {
    return this.pageHaveGuides.asObservable();
  }

  /**
   * Register a guide
   * @param guideName Name, used to identify this specific guide.
   */
  register(component: GuideComponent): void {
    let hasShown = this.settings.hasShown(component.name);
    if (hasShown) {
      return;
    }

    if (component.name === "showGuideTooltip" && this.isLoaded) {
      this.show(component.name, component.title, component.body);
    }

    this.availableGuides.push(component);
    if (this.isLoaded && this.availableGuides.length === 1) {
      this.pageHaveGuides.next(true);
    }
  }

  /**
   * Remove a guide.
   * @param guideName Name, used to identify this specific guide.
   */
  unregister(component: GuideComponent) {
    this.availableGuides = this.availableGuides.filter(x => x.name !== component.name);
    if (this.availableGuides.length === 0) {
      this.pageHaveGuides.next(false);
    }
  }

  showNextGuide() {
    if (this.availableGuides.length === 0) {
      this.pageHaveGuides.next(false);
      return;
    }

    var guide = this.availableGuides[0];
    this.show(guide.name, guide.title, guide.body);
  }

  show(guideName: string, title: string, body: string | HTMLElement) {
    if (!guideName) {
      throw new Error("guideName must be specified.");
    }

    if (!title) {
      throw new Error("title must be specified for " + guideName);
    }

    if (!body) {
      throw new Error("body must be specified for " + guideName);
    }



    this.activeGuide = guideName;
    this.availableGuides = this.availableGuides.filter(x => x.name !== guideName);

    this.settings.shownGuides.push(guideName);
    this.storeSettings();
    this.guideManager.show('#' + guideName, title, body, this.availableGuides.length > 0);
  }

  async canShow(name: string): Promise<boolean> {
    await this.loadHandler.promise;
    return this.activeGuide === "" && !this.settings.hasShown(name);
  }

  async load(): Promise<GuideSettings> {
    const settings = await this.settingsService.get("StoredGuides");
    if (settings) {
      var config = <GuideSettings>JSON.parse(settings);
      this.settings.shownGuides = config.shownGuides;
    }

    // need to filter out shown as they got registered before this load.
    this.availableGuides = this.availableGuides.filter(x => !this.settings.shownGuides.includes(x.name));
    this.pageHaveGuides.next(this.availableGuides.length > 0);

    if (this.canShow("showGuideTooltip")) {
      var component = this.availableGuides.find(x => x.name === "showGuideTooltip");
      if (component) {
        this.show(component.name, component.title, component.body);
      }
    }

    this.isLoaded = true;
    return this.settings;
  }

  private async guideClosed(): Promise<void> {
    if (this.availableGuides.length === 0) {
      this.pageHaveGuides.next(false);
      return;
    }
    this.activeGuide = null;
  }

  private async storeSettings(): Promise<void> {
    const json = JSON.stringify(this.settings);
    await this.settingsService.set("StoredGuides", json);
  }
}
