import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators'
import axios from 'axios'

export interface AssignmentTypeInfoType {
    name: string;
    code: number;
    label: string;
    colorCode: string;
    abbreviation: string;
    // Backward compatibility aliases
    description?: string;
    displayColor?: string;
}

let loadingPromise: Promise<AssignmentTypeInfoType[]> | null = null;

@Module({
  namespaced: true
})
class AssignmentTypesInformation extends VuexModule {

  public assignmentTypes: AssignmentTypeInfoType[] = [];
  public isLoaded = false;

  @Mutation
  public setAssignmentTypes(types: AssignmentTypeInfoType[]): void {
    this.assignmentTypes = types;
    this.isLoaded = true;
  }

  @Mutation
  public clearCache(): void {
    this.assignmentTypes = [];
    this.isLoaded = false;
    loadingPromise = null;
  }

  @Action({ rawError: true })
  public async FetchAssignmentTypes(): Promise<AssignmentTypeInfoType[]> {
    // Return cached data if available
    if (this.isLoaded && this.assignmentTypes.length > 0) {
      return this.assignmentTypes;
    }

    // If a request is already in flight, return the same promise
    if (loadingPromise) {
      return loadingPromise;
    }

    loadingPromise = (async () => {
      try {
        const url = '/api/lookuptype/actives?category=Assignment';
        const response = await axios.get(url);
        
        const types: AssignmentTypeInfoType[] = response.data.map((item: any) => ({
          name: item.name,
          code: item.code,
          label: item.description,
          colorCode: item.displayColor,
          abbreviation: item.abbreviation || '',
          // Backward compatibility
          description: item.description,
          displayColor: item.displayColor
        }));

        this.context.commit('setAssignmentTypes', types);
        return types;
      } catch (error) {
        loadingPromise = null;
        throw error;
      } finally {
        loadingPromise = null;
      }
    })();

    return loadingPromise;
  }

  @Action
  public ClearAssignmentTypesCache(): void {
    this.context.commit('clearCache');
  }

  get getTypes(): AssignmentTypeInfoType[] {
    return this.assignmentTypes;
  }

  get getTypeByCode() {
    return (code: number | string) => {
      return this.assignmentTypes.find(t => 
        t.code.toString() === code.toString()
      );
    }
  }

  get getTypeByName() {
    return (name: string) => {
      return this.assignmentTypes.find(t => 
        t.name.toLowerCase() === name.toLowerCase()
      );
    }
  }

  get getAbbreviations() {
    const abbr: { [key: string]: string } = {};
    this.assignmentTypes.forEach(type => {
      abbr[type.name] = type.abbreviation;
    });
    return abbr;
  }

  get getTypesWithColorCode() {
    const types: { name: string; code: number; colorCode: string }[] = this.assignmentTypes.map(item => ({
      name: item.name,
      code: item.code,
      colorCode: item.displayColor!
    }));
    types.push({ name: 'overtime', code: -1, colorCode: '#e85a0e' });
    types.push({ name: 'free', code: -2, colorCode: '#e6d9e2' });
    return types;
  }

  get getTypeOptions() {
    return this.assignmentTypes.map(item => ({
      name: item.name,
      code: item.code,
      label: item.description!
    }));
  }

  get getDutyColors() {
    const colors = this.assignmentTypes.map(item => ({
      name: item.description!,
      colorCode: item.displayColor!
    }));
    colors.push({ name: 'overtime', colorCode: '#e85a0e' });
    colors.push({ name: 'free', colorCode: '#e6d9e2' });
    return colors;
  }


  get getTypesDetailed() {
    return this.assignmentTypes.map(item => ({
      name: item.name,
      label: item.description!,
      code: item.code,
      colorCode: item.displayColor!
    }));
  }

  get getWindowDutyColors() {
    const colors = this.assignmentTypes.map(item => ({
      name: item.name.toLowerCase(),
      label: item.description!,
      code: item.code,
      colorCode: item.displayColor!
    }));
    colors.push({ name: 'overtime', colorCode: '#e85a0e' } as any);
    colors.push({ name: 'free', colorCode: '#e6d9e2' } as any);
    return colors;
  }

  get getDutyColorsForDayView() {
    const colors: { name: string; code: number; colorCode: string }[] = this.assignmentTypes.map(item => ({
      name: item.description!,
      code: item.code,
      colorCode: item.displayColor!
    }));
    colors.push({ name: 'overtime', code: -1, colorCode: '#e85a0e' });
    colors.push({ name: 'free', code: -2, colorCode: '#e6d9e2' });
    return colors;
  }
}

export default AssignmentTypesInformation
