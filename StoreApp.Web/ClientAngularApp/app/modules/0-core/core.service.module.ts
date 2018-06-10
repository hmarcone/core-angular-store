import { NgModule, Optional, SkipSelf } from '@angular/core';

import { CoreBaseService } from '../../services/0-core/core.base.service';
import { CoreAlertService } from '../../services/0-core/core.alert.service';
import { CoreMenuService } from '../../services/0-core/core.menu.service';


//A module to keep the global services to not mess the AppModule organization
@NgModule({
    providers:
    [
        CoreAlertService,
        CoreBaseService,
        CoreMenuService
    ],
})
export class CoreServiceModule {
    constructor( @Optional() @SkipSelf() core: CoreServiceModule) {
        if (core) {
            throw new Error("CoreServiceModule can not be instantiated by injection");
        }
    }
}