from unityparser import UnityDocument
from enum import Enum
import yaml

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
    checkpoint = '49dc31d8ef8334f17b3451567743481e'
    lava = 'e1858a08e2538704fbb588752fee4e3e'
    spike = 'c5ca291e14223427ca02c6e1181c1d5e'
    door = 'ac57bdd24b4c040218eb99c8a6b95be9'
    triggerBoard = '6cfeb0107473b4df588127baf9804cd5'

numberOfPrefabs = {}
for item in SupportedPrefabGuid:
    numberOfPrefabs[item] = 0
numberOfPrefabs['other'] = 0

document = '/Users/pascerveau/VII/Assets/Scenes/Trevor_Lava.unity'
UnityStreamNoTags = removeUnityTagAlias(document)

ListOfNodes = list()

for data in yaml.load_all(UnityStreamNoTags):
    ListOfNodes.append( data )

filtered = filter(lambda x: 'PrefabInstance' in x, ListOfNodes)

def setKey(dataDict, *names):
    for name in names:
        if name not in dataDict:
            dataDict[name] = None

for node in filtered:
    modifications = node['PrefabInstance']['m_Modification']['m_Modifications']
    dataDict = {}
    for data in modifications:
        if data['value'] != None:
            dataDict[data['propertyPath']] = data['value']
        else:
            dataDict[data['propertyPath']] = data['objectReference']['fileID']
    dataDict['m_SourcePrefab'] = getPrefabGuid(node['PrefabInstance'])
    dataDict['m_InstanceID'] = node['PrefabInstance']['instanceID']
    print(dataDict)
    # floor logic
    if dataDict['m_SourcePrefab'] == SupportedPrefabGuid.floor.value:
        numberOfPrefabs[SupportedPrefabGuid.floor] = numberOfPrefabs[SupportedPrefabGuid.floor] + 1
        setKey(dataDict, 'm_floorState', 'declineAfterExit', 'stepsBeforeIncline')
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
    # lava logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.lava.value:
        numberOfPrefabs[SupportedPrefabGuid.lava] = numberOfPrefabs[SupportedPrefabGuid.lava] + 1
    # spike logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.spike.value:
        numberOfPrefabs[SupportedPrefabGuid.spike] = numberOfPrefabs[SupportedPrefabGuid.spike] + 1
        setKey(dataDict, 'spikeUp')
    # door logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.door.value:
        numberOfPrefabs[SupportedPrefabGuid.door] = numberOfPrefabs[SupportedPrefabGuid.door] + 1
    # triggerBoard logic
    elif dataDict['m_SourcePrefab'] == SupportedPrefabGuid.triggerBoard.value:
        numberOfPrefabs[SupportedPrefabGuid.triggerBoard] = numberOfPrefabs[SupportedPrefabGuid.triggerBoard] + 1
    # other logic
    else:
        numberOfPrefabs['other'] = numberOfPrefabs['other'] + 1
    #print(dataDict)
# Example, print each object's name and type
# for node in ListOfNodes:
#     print(node)
    #if 'm_Name' in node[ node.keys()[0] ]:
        #print( 'Name: ' + node[ node.keys()[0] ]['m_Name']  + ' NodeType: ' + node.keys()[0] )
    #else:
        #print( 'Name: ' + 'No Name Attribute'  + ' NodeType: ' + node.keys()[0] )