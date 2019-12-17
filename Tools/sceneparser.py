from unityparser import UnityDocument
from enum import Enum
import yaml
import json
import os

def removeUnityTagAlias(filepath):
    """
    Name:               removeUnityTagAlias()
    Description:        Loads a file object from a Unity textual scene file, which is in a pseudo YAML style, and strips the
                        parts that are not YAML 1.1 compliant. Then returns a string as a stream, which can be passed to PyYAML.
                        Essentially removes the "!u!" tag directive, class type and the "&" file ID directive. PyYAML seems to handle
                        rest just fine after that.
    Returns:                String (YAML stream as string)  
    """
    result = str()
    sourceFile = open(filepath, 'r')

    fileID = ''
    for lineNumber,line in enumerate( sourceFile.readlines() ): 
        if line.startswith('--- !u!'):
            result += '---' + '\n'         
            fileID = line.split(' ')[2].replace('&', '')   # remove the tag, but keep file ID
        else:
            # Just copy the contents...
            result += line
            if fileID != '':
                result += ('  instanceID: ' + fileID + '\n')
                fileID = ''

    sourceFile.close()  
    
    return result

def getPrefabGuid(prefabInstance):
    return prefabInstance['m_SourcePrefab']['guid']

class SupportedPrefabGuid(Enum):
    floor = '25399a8663513438abe48fcceca3e09e'
    floorConcave = '3b68c9bc9bd2648e8852d4d2b19e4673'
    ice = '08c6a9191fe224a179b16832b8d4ff63'
    iceConcave = '311bfe237ebcc4c2085daad068a2412b'
    wall = 'e3a352e5e460640c99199adbd07d7925'
    trap = '9a04efca5190044af8c9cb52b1da8944'
    checkpoint = '57b4cb67c63b54e09ab4f8a32995c2ef'
    lava = 'e1858a08e2538704fbb588752fee4e3e'
    spike = 'c5ca291e14223427ca02c6e1181c1d5e'
    door = 'ac57bdd24b4c040218eb99c8a6b95be9'
    triggerBoard = '6cfeb0107473b4df588127baf9804cd5'

class TriggerAction(Enum):
    move = 'Move'
    resetPosition = 'ResetPosition'

def tryGetValueFromDict(dict, name, defaultValue = None):
    if name in dict:
        return dict[name]
    else:
        return defaultValue

def moveActionHandler(data, actionName):
    params = {
        'x':tryGetValueFromDict(data, actionName + '.arguments.Array.data[0].vector3Argument.x'),
        'y':tryGetValueFromDict(data, actionName + '.arguments.Array.data[0].vector3Argument.y'),
        'z':tryGetValueFromDict(data, actionName + '.arguments.Array.data[0].vector3Argument.z')
    }
    return params

class EventName(Enum):
    onPlayerEnterEvent = 'OnPlayerEnterEvent'
    onPlayerExitEvent = 'OnPlayerExitEvent'

numberOfPrefabs = {}
for item in SupportedPrefabGuid:
    numberOfPrefabs[item] = 0
numberOfPrefabs['other'] = 0

levelName = 'Trevor_Lava.unity'

document = os.getcwd() + '/Assets/Scenes/' + levelName
UnityStreamNoTags = removeUnityTagAlias(document)

ListOfNodes = list()

for data in yaml.safe_load_all(UnityStreamNoTags):
    ListOfNodes.append( data )

prefabInstances = list(filter(lambda x: 'PrefabInstance' in x, ListOfNodes))
gameObjectInstances = list(filter(lambda x: 'GameObject' in x, ListOfNodes))
transformInstances = list(filter(lambda x: 'Transform' in x, ListOfNodes))

def printInstances(instances=ListOfNodes):
    for item in instances:
        print(item)

def setKey(content, defaultValue=None, *names):
    for name in names:
        if name not in content:
            content[name] = defaultValue

def findNodeByID(instanceID, content=ListOfNodes):
    for item in content:
        target = list(item.values())[0]
        if target['instanceID'] == instanceID:
            return target
    return None

def eventListHandler(content, sEventName, actionList):
    if sEventName + '.persistentCalls.calls.Array.size' not in content:
        return
    for x in range(content[sEventName + '.persistentCalls.calls.Array.size']):
            actionName = sEventName + '.persistentCalls.calls.Array.data[{}]'.format(x)
            if actionName + '.memberName' in content:
                action = {}
                action['event'] = sEventName
                action['target'] = content[actionName + '.target']
                action['memberName'] = content[actionName + '.memberName']
                if action['memberName'] == TriggerAction.move.value:
                    action['params'] = moveActionHandler(content, actionName)
                elif action['memberName'] == TriggerAction.resetPosition:
                    pass
                actionList.append(action)

filterKeys = ['m_Name', 'm_IsActive', 'm_LocalPosition.x', 'm_LocalPosition.y', 'm_LocalPosition.z', 'm_LocalRotation.x', 
    'm_LocalRotation.y', 'm_LocalRotation.z', 'm_LocalRotation.w', 'm_SourcePrefab', 'm_InstanceID', 'm_Attributes', 'm_Actions']

maps = {}
municipalTiles = []
for node in prefabInstances:
    dataDict = {}
    result = {}
    levelTransformID = node['PrefabInstance']['m_Modification']['m_TransformParent']['fileID']
    levelTransform = findNodeByID(levelTransformID, transformInstances)
    levelGameObject = findNodeByID(levelTransform['m_GameObject']['fileID'], gameObjectInstances)
    mapTransform = findNodeByID(levelTransform['m_Father']['fileID'], transformInstances)
    mapGameObject = findNodeByID(mapTransform['m_GameObject']['fileID'], gameObjectInstances)


    if mapGameObject['m_Name'] not in maps:
        maps[mapGameObject['m_Name']] = {}
        maps[mapGameObject['m_Name']][levelGameObject['m_Name']] = []
    elif levelGameObject['m_Name'] not in maps[mapGameObject['m_Name']]:
        maps[mapGameObject['m_Name']][levelGameObject['m_Name']] = []
    maps[mapGameObject['m_Name']][levelGameObject['m_Name']].append(result)

    modifications = node['PrefabInstance']['m_Modification']['m_Modifications']
    for data in modifications:
        if data['value'] != None:
            dataDict[data['propertyPath']] = data['value']
        else:
            dataDict[data['propertyPath']] = findNodeByID(data['objectReference']['fileID'])['m_PrefabInstance']['fileID']
    dataDict['m_SourcePrefab'] = getPrefabGuid(node['PrefabInstance'])
    dataDict['m_InstanceID'] = node['PrefabInstance']['instanceID']
    dataDict['m_Attributes'] = {}
    setKey(dataDict, 1, 'm_IsActive')
    # floor logic
    if dataDict['m_SourcePrefab'] == SupportedPrefabGuid.floor.value:
        numberOfPrefabs[SupportedPrefabGuid.floor] = numberOfPrefabs[SupportedPrefabGuid.floor] + 1
        setKey(dataDict['m_Attributes'], None, 'm_floorState', 'declineAfterExit', 'stepsBeforeIncline')
    # floorConcave logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.floorConcave.value:
        numberOfPrefabs[SupportedPrefabGuid.floorConcave] = numberOfPrefabs[SupportedPrefabGuid.floorConcave] + 1
    # ice logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.ice.value:
        numberOfPrefabs[SupportedPrefabGuid.ice] = numberOfPrefabs[SupportedPrefabGuid.ice] + 1
    # iceConcave logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.iceConcave.value:
        numberOfPrefabs[SupportedPrefabGuid.iceConcave] = numberOfPrefabs[SupportedPrefabGuid.iceConcave] + 1
    # wall logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.wall.value:
        numberOfPrefabs[SupportedPrefabGuid.wall] = numberOfPrefabs[SupportedPrefabGuid.wall] + 1
    # trap logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.trap.value:
        numberOfPrefabs[SupportedPrefabGuid.trap] = numberOfPrefabs[SupportedPrefabGuid.trap] + 1
    # checkpoint logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.checkpoint.value:
        numberOfPrefabs[SupportedPrefabGuid.checkpoint] = numberOfPrefabs[SupportedPrefabGuid.checkpoint] + 1
        setKey(dataDict['m_Attributes'], None, 'respawnObject')
    # lava logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.lava.value:
        numberOfPrefabs[SupportedPrefabGuid.lava] = numberOfPrefabs[SupportedPrefabGuid.lava] + 1
    # spike logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.spike.value:
        numberOfPrefabs[SupportedPrefabGuid.spike] = numberOfPrefabs[SupportedPrefabGuid.spike] + 1
        setKey(dataDict['m_Attributes'], None, 'spikeUp')
    # door logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.door.value:
        numberOfPrefabs[SupportedPrefabGuid.door] = numberOfPrefabs[SupportedPrefabGuid.door] + 1
    # triggerBoard logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.triggerBoard.value:
        numberOfPrefabs[SupportedPrefabGuid.triggerBoard] = numberOfPrefabs[SupportedPrefabGuid.triggerBoard] + 1
        actionList = list()
        eventListHandler(dataDict, EventName.onPlayerEnterEvent.value, actionList)
        eventListHandler(dataDict, EventName.onPlayerExitEvent.value, actionList)
        setKey(dataDict['m_Attributes'], actionList, 'm_Actions')
    # other logic
    else:
        numberOfPrefabs['other'] = numberOfPrefabs['other'] + 1
    for k in filterKeys:
        if k in dataDict:
            result[k] = dataDict[k]
    #print(result)
maps['municipalTiles'] = municipalTiles
print(maps)

outfile = open(os.getcwd() + '/Tools/' + levelName + '.json', 'w')
json.dump(maps, outfile, indent=4)
outfile.close()