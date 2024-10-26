/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./node_modules/idb/build/esm/index.js":
/*!*********************************************!*\
  !*** ./node_modules/idb/build/esm/index.js ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "unwrap": () => /* reexport safe */ _wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.u,
/* harmony export */   "wrap": () => /* reexport safe */ _wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.w,
/* harmony export */   "deleteDB": () => /* binding */ deleteDB,
/* harmony export */   "openDB": () => /* binding */ openDB
/* harmony export */ });
/* harmony import */ var _wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./wrap-idb-value.js */ "./node_modules/idb/build/esm/wrap-idb-value.js");



/**
 * Open a database.
 *
 * @param name Name of the database.
 * @param version Schema version.
 * @param callbacks Additional callbacks.
 */
function openDB(name, version, { blocked, upgrade, blocking, terminated } = {}) {
    const request = indexedDB.open(name, version);
    const openPromise = (0,_wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.w)(request);
    if (upgrade) {
        request.addEventListener('upgradeneeded', (event) => {
            upgrade((0,_wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.w)(request.result), event.oldVersion, event.newVersion, (0,_wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.w)(request.transaction));
        });
    }
    if (blocked)
        request.addEventListener('blocked', () => blocked());
    openPromise
        .then((db) => {
        if (terminated)
            db.addEventListener('close', () => terminated());
        if (blocking)
            db.addEventListener('versionchange', () => blocking());
    })
        .catch(() => { });
    return openPromise;
}
/**
 * Delete a database.
 *
 * @param name Name of the database.
 */
function deleteDB(name, { blocked } = {}) {
    const request = indexedDB.deleteDatabase(name);
    if (blocked)
        request.addEventListener('blocked', () => blocked());
    return (0,_wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.w)(request).then(() => undefined);
}

const readMethods = ['get', 'getKey', 'getAll', 'getAllKeys', 'count'];
const writeMethods = ['put', 'add', 'delete', 'clear'];
const cachedMethods = new Map();
function getMethod(target, prop) {
    if (!(target instanceof IDBDatabase &&
        !(prop in target) &&
        typeof prop === 'string')) {
        return;
    }
    if (cachedMethods.get(prop))
        return cachedMethods.get(prop);
    const targetFuncName = prop.replace(/FromIndex$/, '');
    const useIndex = prop !== targetFuncName;
    const isWrite = writeMethods.includes(targetFuncName);
    if (
    // Bail if the target doesn't exist on the target. Eg, getAll isn't in Edge.
    !(targetFuncName in (useIndex ? IDBIndex : IDBObjectStore).prototype) ||
        !(isWrite || readMethods.includes(targetFuncName))) {
        return;
    }
    const method = async function (storeName, ...args) {
        // isWrite ? 'readwrite' : undefined gzipps better, but fails in Edge :(
        const tx = this.transaction(storeName, isWrite ? 'readwrite' : 'readonly');
        let target = tx.store;
        if (useIndex)
            target = target.index(args.shift());
        const returnVal = await target[targetFuncName](...args);
        if (isWrite)
            await tx.done;
        return returnVal;
    };
    cachedMethods.set(prop, method);
    return method;
}
(0,_wrap_idb_value_js__WEBPACK_IMPORTED_MODULE_0__.r)((oldTraps) => ({
    ...oldTraps,
    get: (target, prop, receiver) => getMethod(target, prop) || oldTraps.get(target, prop, receiver),
    has: (target, prop) => !!getMethod(target, prop) || oldTraps.has(target, prop),
}));




/***/ }),

/***/ "./node_modules/idb/build/esm/wrap-idb-value.js":
/*!******************************************************!*\
  !*** ./node_modules/idb/build/esm/wrap-idb-value.js ***!
  \******************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "a": () => /* binding */ reverseTransformCache,
/* harmony export */   "i": () => /* binding */ instanceOfAny,
/* harmony export */   "r": () => /* binding */ replaceTraps,
/* harmony export */   "u": () => /* binding */ unwrap,
/* harmony export */   "w": () => /* binding */ wrap
/* harmony export */ });
const instanceOfAny = (object, constructors) => constructors.some((c) => object instanceof c);

let idbProxyableTypes;
let cursorAdvanceMethods;
// This is a function to prevent it throwing up in node environments.
function getIdbProxyableTypes() {
    return (idbProxyableTypes ||
        (idbProxyableTypes = [
            IDBDatabase,
            IDBObjectStore,
            IDBIndex,
            IDBCursor,
            IDBTransaction,
        ]));
}
// This is a function to prevent it throwing up in node environments.
function getCursorAdvanceMethods() {
    return (cursorAdvanceMethods ||
        (cursorAdvanceMethods = [
            IDBCursor.prototype.advance,
            IDBCursor.prototype.continue,
            IDBCursor.prototype.continuePrimaryKey,
        ]));
}
const cursorRequestMap = new WeakMap();
const transactionDoneMap = new WeakMap();
const transactionStoreNamesMap = new WeakMap();
const transformCache = new WeakMap();
const reverseTransformCache = new WeakMap();
function promisifyRequest(request) {
    const promise = new Promise((resolve, reject) => {
        const unlisten = () => {
            request.removeEventListener('success', success);
            request.removeEventListener('error', error);
        };
        const success = () => {
            resolve(wrap(request.result));
            unlisten();
        };
        const error = () => {
            reject(request.error);
            unlisten();
        };
        request.addEventListener('success', success);
        request.addEventListener('error', error);
    });
    promise
        .then((value) => {
        // Since cursoring reuses the IDBRequest (*sigh*), we cache it for later retrieval
        // (see wrapFunction).
        if (value instanceof IDBCursor) {
            cursorRequestMap.set(value, request);
        }
        // Catching to avoid "Uncaught Promise exceptions"
    })
        .catch(() => { });
    // This mapping exists in reverseTransformCache but doesn't doesn't exist in transformCache. This
    // is because we create many promises from a single IDBRequest.
    reverseTransformCache.set(promise, request);
    return promise;
}
function cacheDonePromiseForTransaction(tx) {
    // Early bail if we've already created a done promise for this transaction.
    if (transactionDoneMap.has(tx))
        return;
    const done = new Promise((resolve, reject) => {
        const unlisten = () => {
            tx.removeEventListener('complete', complete);
            tx.removeEventListener('error', error);
            tx.removeEventListener('abort', error);
        };
        const complete = () => {
            resolve();
            unlisten();
        };
        const error = () => {
            reject(tx.error || new DOMException('AbortError', 'AbortError'));
            unlisten();
        };
        tx.addEventListener('complete', complete);
        tx.addEventListener('error', error);
        tx.addEventListener('abort', error);
    });
    // Cache it for later retrieval.
    transactionDoneMap.set(tx, done);
}
let idbProxyTraps = {
    get(target, prop, receiver) {
        if (target instanceof IDBTransaction) {
            // Special handling for transaction.done.
            if (prop === 'done')
                return transactionDoneMap.get(target);
            // Polyfill for objectStoreNames because of Edge.
            if (prop === 'objectStoreNames') {
                return target.objectStoreNames || transactionStoreNamesMap.get(target);
            }
            // Make tx.store return the only store in the transaction, or undefined if there are many.
            if (prop === 'store') {
                return receiver.objectStoreNames[1]
                    ? undefined
                    : receiver.objectStore(receiver.objectStoreNames[0]);
            }
        }
        // Else transform whatever we get back.
        return wrap(target[prop]);
    },
    set(target, prop, value) {
        target[prop] = value;
        return true;
    },
    has(target, prop) {
        if (target instanceof IDBTransaction &&
            (prop === 'done' || prop === 'store')) {
            return true;
        }
        return prop in target;
    },
};
function replaceTraps(callback) {
    idbProxyTraps = callback(idbProxyTraps);
}
function wrapFunction(func) {
    // Due to expected object equality (which is enforced by the caching in `wrap`), we
    // only create one new func per func.
    // Edge doesn't support objectStoreNames (booo), so we polyfill it here.
    if (func === IDBDatabase.prototype.transaction &&
        !('objectStoreNames' in IDBTransaction.prototype)) {
        return function (storeNames, ...args) {
            const tx = func.call(unwrap(this), storeNames, ...args);
            transactionStoreNamesMap.set(tx, storeNames.sort ? storeNames.sort() : [storeNames]);
            return wrap(tx);
        };
    }
    // Cursor methods are special, as the behaviour is a little more different to standard IDB. In
    // IDB, you advance the cursor and wait for a new 'success' on the IDBRequest that gave you the
    // cursor. It's kinda like a promise that can resolve with many values. That doesn't make sense
    // with real promises, so each advance methods returns a new promise for the cursor object, or
    // undefined if the end of the cursor has been reached.
    if (getCursorAdvanceMethods().includes(func)) {
        return function (...args) {
            // Cgensysg the original function with the proxy as 'this' causes ILLEGAL INVOCATION, so we use
            // the original object.
            func.apply(unwrap(this), args);
            return wrap(cursorRequestMap.get(this));
        };
    }
    return function (...args) {
        // Cgensysg the original function with the proxy as 'this' causes ILLEGAL INVOCATION, so we use
        // the original object.
        return wrap(func.apply(unwrap(this), args));
    };
}
function transformCachableValue(value) {
    if (typeof value === 'function')
        return wrapFunction(value);
    // This doesn't return, it just creates a 'done' promise for the transaction,
    // which is later returned for transaction.done (see idbObjectHandler).
    if (value instanceof IDBTransaction)
        cacheDonePromiseForTransaction(value);
    if (instanceOfAny(value, getIdbProxyableTypes()))
        return new Proxy(value, idbProxyTraps);
    // Return the same value back if we're not going to transform it.
    return value;
}
function wrap(value) {
    // We sometimes generate multiple promises from a single IDBRequest (eg when cursoring), because
    // IDB is weird and a single IDBRequest can yield many responses, so these can't be cached.
    if (value instanceof IDBRequest)
        return promisifyRequest(value);
    // If we've already transformed this value before, reuse the transformed value.
    // This is faster, but it also provides object equality.
    if (transformCache.has(value))
        return transformCache.get(value);
    const newValue = transformCachableValue(value);
    // Not all types are transformed.
    // These may be primitive types, so they can't be WeakMap keys.
    if (newValue !== value) {
        transformCache.set(value, newValue);
        reverseTransformCache.set(newValue, value);
    }
    return newValue;
}
const unwrap = (value) => reverseTransformCache.get(value);




/***/ }),

/***/ "./client/InitialiseIndexDbBlazor.ts":
/*!*******************************************!*\
  !*** ./client/InitialiseIndexDbBlazor.ts ***!
  \*******************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
const indexedDbBlazor_1 = __webpack_require__(/*! ./indexedDbBlazor */ "./client/indexedDbBlazor.ts");
var IndexDb;
(function (IndexDb) {
    const timeghostExtensions = 'BlazorIndexedDbJs';
    const extensionObject = {
        IDBManager: new indexedDbBlazor_1.IndexedDbManager()
    };
    function initialise() {
        if (typeof window !== 'undefined' && !window[timeghostExtensions]) {
            window[timeghostExtensions] = Object.assign({}, extensionObject);
        }
        else {
            window[timeghostExtensions] = Object.assign(Object.assign({}, window[timeghostExtensions]), extensionObject);
        }
    }
    IndexDb.initialise = initialise;
})(IndexDb || (IndexDb = {}));
IndexDb.initialise();


/***/ }),

/***/ "./client/indexedDbBlazor.ts":
/*!***********************************!*\
  !*** ./client/indexedDbBlazor.ts ***!
  \***********************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.IndexedDbManager = void 0;
const idb_1 = __webpack_require__(/*! idb */ "./node_modules/idb/build/esm/index.js");
const E_DB_CLOSED = "Database is closed";
class IndexedDbManager {
    constructor() {
        this.dbInstance = undefined;
        this.open = (database) => __awaiter(this, void 0, void 0, function* () {
            var upgradeError = "";
            try {
                if (!this.dbInstance || this.dbInstance.version < database.version) {
                    if (this.dbInstance) {
                        this.dbInstance.close();
                        this.dbInstance = undefined;
                    }
                    this.dbInstance = yield idb_1.openDB(database.name, database.version, {
                        upgrade(db, oldVersion, newVersion, transaction) {
                            try {
                                IndexedDbManager.upgradeDatabase(db, oldVersion, newVersion, database);
                            }
                            catch (error) {
                                upgradeError = error.toString();
                                throw (error);
                            }
                        },
                    });
                }
            }
            catch (error) {
                throw error.toString() + ' ' + upgradeError;
            }
        });
        this.deleteDatabase = (dbName) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                this.dbInstance.close();
                yield idb_1.deleteDB(dbName);
                this.dbInstance = undefined;
            }
            catch (error) {
                throw `Database ${dbName}, ${error.toString()}`;
            }
        });
        this.getDbSchema = (dbName) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const dbInstance = this.dbInstance;
                const dbInfo = {
                    name: dbInstance.name,
                    version: dbInstance.version,
                    objectStores: []
                };
                for (let s = 0; s < dbInstance.objectStoreNames.length; s++) {
                    let dbStore = dbInstance.transaction(dbInstance.objectStoreNames[s], 'readonly').store;
                    let objectStore = {
                        name: dbStore.name,
                        keyPath: Array.isArray(dbStore.keyPath) ? dbStore.keyPath.join(',') : dbStore.keyPath,
                        autoIncrement: dbStore.autoIncrement,
                        indexes: []
                    };
                    for (let i = 0; i < dbStore.indexNames.length; i++) {
                        const dbIndex = dbStore.index(dbStore.indexNames[i]);
                        let index = {
                            name: dbIndex.name,
                            keyPath: Array.isArray(dbIndex.keyPath) ? dbIndex.keyPath.join(',') : dbIndex.keyPath,
                            multiEntry: dbIndex.multiEntry,
                            unique: dbIndex.unique
                        };
                        objectStore.indexes.push(index);
                    }
                    dbInfo.objectStores.push(objectStore);
                }
                return dbInfo;
            }
            catch (error) {
                throw `Database ${dbName}, ${error.toString()}`;
            }
        });
        this.count = (storeName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let result = yield tx.store.count(key !== null && key !== void 0 ? key : undefined);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.countByKeyRange = (storeName, lower, upper, lowerOpen, upperOpen) => __awaiter(this, void 0, void 0, function* () {
            try {
                return yield this.count(storeName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen));
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.get = (storeName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let result = yield tx.store.get(key);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAll = (storeName, key, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let results = yield tx.store.getAll(key !== null && key !== void 0 ? key : undefined, count !== null && count !== void 0 ? count : undefined);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAllByKeyRange = (storeName, lower, upper, lowerOpen, upperOpen, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                return yield this.getAll(storeName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen), count);
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAllByArrayKey = (storeName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let results = [];
                for (let index = 0; index < key.length; index++) {
                    const element = key[index];
                    results = results.concat(yield tx.store.getAll(element));
                }
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getKey = (storeName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let result = yield tx.store.getKey(key);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAllKeys = (storeName, key, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let results = yield tx.store.getAllKeys(key !== null && key !== void 0 ? key : undefined, count !== null && count !== void 0 ? count : undefined);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAllKeysByKeyRange = (storeName, lower, upper, lowerOpen, upperOpen, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                return yield this.getAllKeys(storeName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen), count);
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.getAllKeysByArrayKey = (storeName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let results = [];
                for (let index = 0; index < key.length; index++) {
                    const element = key[index];
                    results = results.concat(yield tx.store.getAllKeys(element));
                }
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.query = (storeName, key, filter, count = 0, skip = 0) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                try {
                    var func = new Function('obj', filter);
                }
                catch (error) {
                    throw `${error.toString()} in filter { ${filter} }`;
                }
                var row = 0;
                var errorMessage = "";
                let results = [];
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let cursor = yield tx.store.openCursor(key !== null && key !== void 0 ? key : undefined);
                while (cursor) {
                    if (!cursor) {
                        return;
                    }
                    try {
                        var out = func(cursor.value);
                        if (out) {
                            row++;
                            if (row > skip) {
                                results.push(out);
                            }
                        }
                    }
                    catch (error) {
                        errorMessage = `obj: ${JSON.stringify(cursor.value)}\nfilter: ${filter}\nerror: ${error.toString()}`;
                        return;
                    }
                    if (count > 0 && results.length >= count) {
                        return;
                    }
                    cursor = yield cursor.continue();
                }
                yield tx.done;
                if (errorMessage) {
                    throw errorMessage;
                }
                return results;
            }
            catch (error) {
                throw `Store ${storeName} ${error.toString()}`;
            }
        });
        this.countFromIndex = (storeName, indexName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                let result = yield tx.store.index(indexName).count(key !== null && key !== void 0 ? key : undefined);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.countFromIndexByKeyRange = (storeName, indexName, lower, upper, lowerOpen, upperOpen) => __awaiter(this, void 0, void 0, function* () {
            try {
                return yield this.countFromIndex(storeName, indexName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen));
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getFromIndex = (storeName, indexName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const results = yield tx.store.index(indexName).get(key);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllFromIndex = (storeName, indexName, key, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const results = yield tx.store.index(indexName).getAll(key !== null && key !== void 0 ? key : undefined, count !== null && count !== void 0 ? count : undefined);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllFromIndexByKeyRange = (storeName, indexName, lower, upper, lowerOpen, upperOpen, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                return yield this.getAllFromIndex(storeName, indexName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen), count);
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllFromIndexByArrayKey = (storeName, indexName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const dx = tx.store.index(indexName);
                let results = [];
                for (let index = 0; index < key.length; index++) {
                    const element = key[index];
                    results = results.concat(yield dx.getAll(element));
                }
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getKeyFromIndex = (storeName, indexName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const results = yield tx.store.index(indexName).getKey(key);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllKeysFromIndex = (storeName, indexName, key, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const results = yield tx.store.index(indexName).getAllKeys(key !== null && key !== void 0 ? key : undefined, count !== null && count !== void 0 ? count : undefined);
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllKeysFromIndexByKeyRange = (storeName, indexName, lower, upper, lowerOpen, upperOpen, count) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                return yield this.getAllKeysFromIndex(storeName, indexName, IDBKeyRange.bound(lower, upper, lowerOpen, upperOpen), count);
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.getAllKeysFromIndexByArrayKey = (storeName, indexName, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readonly');
                const dx = tx.store.index(indexName);
                let results = [];
                for (let index = 0; index < key.length; index++) {
                    const element = key[index];
                    results = results.concat(yield dx.getAllKeys(element));
                }
                yield tx.done;
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName}, ${error.toString()}`;
            }
        });
        this.queryFromIndex = (storeName, indexName, key, filter, count = 0, skip = 0,order="next") => __awaiter(this, void 0, void 0, function* () {
            try {
                console.log(key);
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                if (filter) {
                    try {
                        var func = new Function('obj', filter);
                    }
                    catch (error) {
                        throw `${error.toString()} in filter { ${filter} }`;
                    }
                }
                var row = 0;
                var errorMessage = "";
                let results = [];
                const tx = this.dbInstance.transaction(storeName, 'readonly');
             
                var range = IDBKeyRange.bound(key.lower, key.upper, key.lowerOpen, key.upperOpen);

                let cursor = yield tx.store.index(indexName).openCursor(range,order);
                while (cursor) {
                    if (!cursor) {
                        return;
                    }
                    try {
                        var out = cursor.value;
                        if (filter) {
                            out = func(out);
                        }
                        if (out) {
                            row++;
                            if (row > skip) {
                                results.push(out);
                            }
                        }
                    }
                    catch (error) {
                        errorMessage = `obj: ${JSON.stringify(cursor.value)}\nfilter: ${filter}\nerror: ${error.toString()}`;
                        console.log(errorMessage);
                        return;
                    }
                    if (count > 0 && results.length >= count) {
                        return results;
                    }
                    cursor = yield cursor.continue();
                }
                yield tx.done;
                if (errorMessage) {
                    throw errorMessage;
                }
                return results;
            }
            catch (error) {
                throw `Store ${storeName}, Index ${indexName},range(${range.lower},${range.upper},${range.lowerOpen},${range.upperOpen} ),order:${order},${error.toString()}`;
            }
        });
        this.add = (storeName, data, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                data = this.checkForKeyPath(tx.store, data);
                const result = yield tx.store.add(data, key !== null && key !== void 0 ? key : undefined);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.put = (storeName, data, key) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                const result = yield tx.store.put(data, key !== null && key !== void 0 ? key : undefined);
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.delete = (storeName, id) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                yield tx.store.delete(id);
                yield tx.done;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.batchAdd = (storeName, data) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                let result = [];
                data.forEach((element) => __awaiter(this, void 0, void 0, function* () {
                    let item = this.checkForKeyPath(tx.store, element);
                    result.push(yield tx.store.add(item));
                }));
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.batchPut = (storeName, data) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                let result = [];
                data.forEach((element) => __awaiter(this, void 0, void 0, function* () {
                    result.push(yield tx.store.put(element));
                }));
                yield tx.done;
                return result;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.batchDelete = (storeName, ids) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                ids.forEach((element) => __awaiter(this, void 0, void 0, function* () {
                    yield tx.store.delete(element);
                }));
                yield tx.done;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
        this.clearStore = (storeName) => __awaiter(this, void 0, void 0, function* () {
            try {
                if (!this.dbInstance)
                    throw E_DB_CLOSED;
                const tx = this.dbInstance.transaction(storeName, 'readwrite');
                yield tx.store.clear();
                yield tx.done;
            }
            catch (error) {
                throw `Store ${storeName}, ${error.toString()}`;
            }
        });
    }
    checkForKeyPath(objectStore, data) {
        if (!objectStore.autoIncrement || !objectStore.keyPath) {
            return data;
        }
        if (typeof objectStore.keyPath !== 'string') {
            return data;
        }
        const keyPath = objectStore.keyPath;
        if (!data[keyPath]) {
            delete data[keyPath];
        }
        return data;
    }
    static upgradeDatabase(upgradeDB, oldVersion, newVersion, dbDatabase) {
        if (newVersion && newVersion > oldVersion) {
            if (dbDatabase.objectStores) {
                for (var store of dbDatabase.objectStores) {
                    if (!upgradeDB.objectStoreNames.contains(store.name)) {
                        this.addNewStore(upgradeDB, store);
                    }
                }
            }
        }
    }
    static getKeyPath(keyPath) {
        if (keyPath) {
            var multiKeyPath = keyPath.split(',');
            return multiKeyPath.length > 1 ? multiKeyPath : keyPath;
        }
        else {
            return undefined;
        }
    }
    static addNewStore(upgradeDB, store) {
        var _a;
        try {
            const newStore = upgradeDB.createObjectStore(store.name, {
                keyPath: this.getKeyPath(store.keyPath),
                autoIncrement: store.autoIncrement
            });
            for (var index of store.indexes) {
                try {
                    newStore.createIndex(index.name, (_a = this.getKeyPath(index.keyPath)) !== null && _a !== void 0 ? _a : index.name, {
                        multiEntry: index.multiEntry,
                        unique: index.unique
                    });
                }
                catch (error) {
                    throw `index ${index.name}, ${error.toString()}`;
                }
            }
        }
        catch (error) {
            throw `store ${store.name}, ${error.toString()}`;
        }
    }
}
exports.IndexedDbManager = IndexedDbManager;


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		if(__webpack_module_cache__[moduleId]) {
/******/ 			return __webpack_module_cache__[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */ 
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => Object.prototype.hasOwnProperty.call(obj, prop)
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	// startup
/******/ 	// Load entry module
/******/ 	__webpack_require__("./client/InitialiseIndexDbBlazor.ts");
/******/ 	// This entry module used 'exports' so it can't be inlined
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvLi9ub2RlX21vZHVsZXMvaWRiL2J1aWxkL2VzbS9pbmRleC5qcyIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvLi9ub2RlX21vZHVsZXMvaWRiL2J1aWxkL2VzbS93cmFwLWlkYi12YWx1ZS5qcyIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvLi9jbGllbnQvSW5pdGlhbGlzZUluZGV4RGJCbGF6b3IudHMiLCJ3ZWJwYWNrOi8vaW5kZXhlZGRiYmxhem9yLmpzLy4vY2xpZW50L2luZGV4ZWREYkJsYXpvci50cyIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5kZXhlZGRiYmxhem9yLmpzL3dlYnBhY2svcnVudGltZS9kZWZpbmUgcHJvcGVydHkgZ2V0dGVycyIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvd2VicGFjay9ydW50aW1lL2hhc093blByb3BlcnR5IHNob3J0aGFuZCIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvd2VicGFjay9ydW50aW1lL21ha2UgbmFtZXNwYWNlIG9iamVjdCIsIndlYnBhY2s6Ly9pbmRleGVkZGJibGF6b3IuanMvd2VicGFjay9zdGFydHVwIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUFtRTtBQUNOOztBQUU3RDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGdDQUFnQyx5Q0FBeUMsS0FBSztBQUM5RTtBQUNBLHdCQUF3QixxREFBSTtBQUM1QjtBQUNBO0FBQ0Esb0JBQW9CLHFEQUFJLHNEQUFzRCxxREFBSTtBQUNsRixTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMLHNCQUFzQixFQUFFO0FBQ3hCO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EseUJBQXlCLFVBQVUsS0FBSztBQUN4QztBQUNBO0FBQ0E7QUFDQSxXQUFXLHFEQUFJO0FBQ2Y7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxxREFBWTtBQUNaO0FBQ0E7QUFDQTtBQUNBLENBQUM7O0FBRTJCOzs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FDbEY1Qjs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLEtBQUs7QUFDTDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMLHNCQUFzQixFQUFFO0FBQ3hCO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxLQUFLO0FBQ0w7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxLQUFLO0FBQ0w7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMO0FBQ0E7QUFDQTtBQUNB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QTtXQUNBO1dBQ0E7V0FDQSxzREFBc0Qsa0JBQWtCO1dBQ3hFO1dBQ0EsK0NBQStDLGNBQWM7V0FDN0QsRTs7OztVQ05BO1VBQ0E7VUFDQTtVQUNBIiwiZmlsZSI6IkJsYXpvckluZGV4ZWREYi5qcyIsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCB7IHcgYXMgd3JhcCwgciBhcyByZXBsYWNlVHJhcHMgfSBmcm9tICcuL3dyYXAtaWRiLXZhbHVlLmpzJztcbmV4cG9ydCB7IHUgYXMgdW53cmFwLCB3IGFzIHdyYXAgfSBmcm9tICcuL3dyYXAtaWRiLXZhbHVlLmpzJztcblxuLyoqXG4gKiBPcGVuIGEgZGF0YWJhc2UuXG4gKlxuICogQHBhcmFtIG5hbWUgTmFtZSBvZiB0aGUgZGF0YWJhc2UuXG4gKiBAcGFyYW0gdmVyc2lvbiBTY2hlbWEgdmVyc2lvbi5cbiAqIEBwYXJhbSBjYWxsYmFja3MgQWRkaXRpb25hbCBjYWxsYmFja3MuXG4gKi9cbmZ1bmN0aW9uIG9wZW5EQihuYW1lLCB2ZXJzaW9uLCB7IGJsb2NrZWQsIHVwZ3JhZGUsIGJsb2NraW5nLCB0ZXJtaW5hdGVkIH0gPSB7fSkge1xuICAgIGNvbnN0IHJlcXVlc3QgPSBpbmRleGVkREIub3BlbihuYW1lLCB2ZXJzaW9uKTtcbiAgICBjb25zdCBvcGVuUHJvbWlzZSA9IHdyYXAocmVxdWVzdCk7XG4gICAgaWYgKHVwZ3JhZGUpIHtcbiAgICAgICAgcmVxdWVzdC5hZGRFdmVudExpc3RlbmVyKCd1cGdyYWRlbmVlZGVkJywgKGV2ZW50KSA9PiB7XG4gICAgICAgICAgICB1cGdyYWRlKHdyYXAocmVxdWVzdC5yZXN1bHQpLCBldmVudC5vbGRWZXJzaW9uLCBldmVudC5uZXdWZXJzaW9uLCB3cmFwKHJlcXVlc3QudHJhbnNhY3Rpb24pKTtcbiAgICAgICAgfSk7XG4gICAgfVxuICAgIGlmIChibG9ja2VkKVxuICAgICAgICByZXF1ZXN0LmFkZEV2ZW50TGlzdGVuZXIoJ2Jsb2NrZWQnLCAoKSA9PiBibG9ja2VkKCkpO1xuICAgIG9wZW5Qcm9taXNlXG4gICAgICAgIC50aGVuKChkYikgPT4ge1xuICAgICAgICBpZiAodGVybWluYXRlZClcbiAgICAgICAgICAgIGRiLmFkZEV2ZW50TGlzdGVuZXIoJ2Nsb3NlJywgKCkgPT4gdGVybWluYXRlZCgpKTtcbiAgICAgICAgaWYgKGJsb2NraW5nKVxuICAgICAgICAgICAgZGIuYWRkRXZlbnRMaXN0ZW5lcigndmVyc2lvbmNoYW5nZScsICgpID0+IGJsb2NraW5nKCkpO1xuICAgIH0pXG4gICAgICAgIC5jYXRjaCgoKSA9PiB7IH0pO1xuICAgIHJldHVybiBvcGVuUHJvbWlzZTtcbn1cbi8qKlxuICogRGVsZXRlIGEgZGF0YWJhc2UuXG4gKlxuICogQHBhcmFtIG5hbWUgTmFtZSBvZiB0aGUgZGF0YWJhc2UuXG4gKi9cbmZ1bmN0aW9uIGRlbGV0ZURCKG5hbWUsIHsgYmxvY2tlZCB9ID0ge30pIHtcbiAgICBjb25zdCByZXF1ZXN0ID0gaW5kZXhlZERCLmRlbGV0ZURhdGFiYXNlKG5hbWUpO1xuICAgIGlmIChibG9ja2VkKVxuICAgICAgICByZXF1ZXN0LmFkZEV2ZW50TGlzdGVuZXIoJ2Jsb2NrZWQnLCAoKSA9PiBibG9ja2VkKCkpO1xuICAgIHJldHVybiB3cmFwKHJlcXVlc3QpLnRoZW4oKCkgPT4gdW5kZWZpbmVkKTtcbn1cblxuY29uc3QgcmVhZE1ldGhvZHMgPSBbJ2dldCcsICdnZXRLZXknLCAnZ2V0QWxsJywgJ2dldEFsbEtleXMnLCAnY291bnQnXTtcbmNvbnN0IHdyaXRlTWV0aG9kcyA9IFsncHV0JywgJ2FkZCcsICdkZWxldGUnLCAnY2xlYXInXTtcbmNvbnN0IGNhY2hlZE1ldGhvZHMgPSBuZXcgTWFwKCk7XG5mdW5jdGlvbiBnZXRNZXRob2QodGFyZ2V0LCBwcm9wKSB7XG4gICAgaWYgKCEodGFyZ2V0IGluc3RhbmNlb2YgSURCRGF0YWJhc2UgJiZcbiAgICAgICAgIShwcm9wIGluIHRhcmdldCkgJiZcbiAgICAgICAgdHlwZW9mIHByb3AgPT09ICdzdHJpbmcnKSkge1xuICAgICAgICByZXR1cm47XG4gICAgfVxuICAgIGlmIChjYWNoZWRNZXRob2RzLmdldChwcm9wKSlcbiAgICAgICAgcmV0dXJuIGNhY2hlZE1ldGhvZHMuZ2V0KHByb3ApO1xuICAgIGNvbnN0IHRhcmdldEZ1bmNOYW1lID0gcHJvcC5yZXBsYWNlKC9Gcm9tSW5kZXgkLywgJycpO1xuICAgIGNvbnN0IHVzZUluZGV4ID0gcHJvcCAhPT0gdGFyZ2V0RnVuY05hbWU7XG4gICAgY29uc3QgaXNXcml0ZSA9IHdyaXRlTWV0aG9kcy5pbmNsdWRlcyh0YXJnZXRGdW5jTmFtZSk7XG4gICAgaWYgKFxuICAgIC8vIEJhaWwgaWYgdGhlIHRhcmdldCBkb2Vzbid0IGV4aXN0IG9uIHRoZSB0YXJnZXQuIEVnLCBnZXRBbGwgaXNuJ3QgaW4gRWRnZS5cbiAgICAhKHRhcmdldEZ1bmNOYW1lIGluICh1c2VJbmRleCA/IElEQkluZGV4IDogSURCT2JqZWN0U3RvcmUpLnByb3RvdHlwZSkgfHxcbiAgICAgICAgIShpc1dyaXRlIHx8IHJlYWRNZXRob2RzLmluY2x1ZGVzKHRhcmdldEZ1bmNOYW1lKSkpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cbiAgICBjb25zdCBtZXRob2QgPSBhc3luYyBmdW5jdGlvbiAoc3RvcmVOYW1lLCAuLi5hcmdzKSB7XG4gICAgICAgIC8vIGlzV3JpdGUgPyAncmVhZHdyaXRlJyA6IHVuZGVmaW5lZCBnemlwcHMgYmV0dGVyLCBidXQgZmFpbHMgaW4gRWRnZSA6KFxuICAgICAgICBjb25zdCB0eCA9IHRoaXMudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCBpc1dyaXRlID8gJ3JlYWR3cml0ZScgOiAncmVhZG9ubHknKTtcbiAgICAgICAgbGV0IHRhcmdldCA9IHR4LnN0b3JlO1xuICAgICAgICBpZiAodXNlSW5kZXgpXG4gICAgICAgICAgICB0YXJnZXQgPSB0YXJnZXQuaW5kZXgoYXJncy5zaGlmdCgpKTtcbiAgICAgICAgY29uc3QgcmV0dXJuVmFsID0gYXdhaXQgdGFyZ2V0W3RhcmdldEZ1bmNOYW1lXSguLi5hcmdzKTtcbiAgICAgICAgaWYgKGlzV3JpdGUpXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuICAgICAgICByZXR1cm4gcmV0dXJuVmFsO1xuICAgIH07XG4gICAgY2FjaGVkTWV0aG9kcy5zZXQocHJvcCwgbWV0aG9kKTtcbiAgICByZXR1cm4gbWV0aG9kO1xufVxucmVwbGFjZVRyYXBzKChvbGRUcmFwcykgPT4gKHtcbiAgICAuLi5vbGRUcmFwcyxcbiAgICBnZXQ6ICh0YXJnZXQsIHByb3AsIHJlY2VpdmVyKSA9PiBnZXRNZXRob2QodGFyZ2V0LCBwcm9wKSB8fCBvbGRUcmFwcy5nZXQodGFyZ2V0LCBwcm9wLCByZWNlaXZlciksXG4gICAgaGFzOiAodGFyZ2V0LCBwcm9wKSA9PiAhIWdldE1ldGhvZCh0YXJnZXQsIHByb3ApIHx8IG9sZFRyYXBzLmhhcyh0YXJnZXQsIHByb3ApLFxufSkpO1xuXG5leHBvcnQgeyBkZWxldGVEQiwgb3BlbkRCIH07XG4iLCJjb25zdCBpbnN0YW5jZU9mQW55ID0gKG9iamVjdCwgY29uc3RydWN0b3JzKSA9PiBjb25zdHJ1Y3RvcnMuc29tZSgoYykgPT4gb2JqZWN0IGluc3RhbmNlb2YgYyk7XG5cbmxldCBpZGJQcm94eWFibGVUeXBlcztcbmxldCBjdXJzb3JBZHZhbmNlTWV0aG9kcztcbi8vIFRoaXMgaXMgYSBmdW5jdGlvbiB0byBwcmV2ZW50IGl0IHRocm93aW5nIHVwIGluIG5vZGUgZW52aXJvbm1lbnRzLlxuZnVuY3Rpb24gZ2V0SWRiUHJveHlhYmxlVHlwZXMoKSB7XG4gICAgcmV0dXJuIChpZGJQcm94eWFibGVUeXBlcyB8fFxuICAgICAgICAoaWRiUHJveHlhYmxlVHlwZXMgPSBbXG4gICAgICAgICAgICBJREJEYXRhYmFzZSxcbiAgICAgICAgICAgIElEQk9iamVjdFN0b3JlLFxuICAgICAgICAgICAgSURCSW5kZXgsXG4gICAgICAgICAgICBJREJDdXJzb3IsXG4gICAgICAgICAgICBJREJUcmFuc2FjdGlvbixcbiAgICAgICAgXSkpO1xufVxuLy8gVGhpcyBpcyBhIGZ1bmN0aW9uIHRvIHByZXZlbnQgaXQgdGhyb3dpbmcgdXAgaW4gbm9kZSBlbnZpcm9ubWVudHMuXG5mdW5jdGlvbiBnZXRDdXJzb3JBZHZhbmNlTWV0aG9kcygpIHtcbiAgICByZXR1cm4gKGN1cnNvckFkdmFuY2VNZXRob2RzIHx8XG4gICAgICAgIChjdXJzb3JBZHZhbmNlTWV0aG9kcyA9IFtcbiAgICAgICAgICAgIElEQkN1cnNvci5wcm90b3R5cGUuYWR2YW5jZSxcbiAgICAgICAgICAgIElEQkN1cnNvci5wcm90b3R5cGUuY29udGludWUsXG4gICAgICAgICAgICBJREJDdXJzb3IucHJvdG90eXBlLmNvbnRpbnVlUHJpbWFyeUtleSxcbiAgICAgICAgXSkpO1xufVxuY29uc3QgY3Vyc29yUmVxdWVzdE1hcCA9IG5ldyBXZWFrTWFwKCk7XG5jb25zdCB0cmFuc2FjdGlvbkRvbmVNYXAgPSBuZXcgV2Vha01hcCgpO1xuY29uc3QgdHJhbnNhY3Rpb25TdG9yZU5hbWVzTWFwID0gbmV3IFdlYWtNYXAoKTtcbmNvbnN0IHRyYW5zZm9ybUNhY2hlID0gbmV3IFdlYWtNYXAoKTtcbmNvbnN0IHJldmVyc2VUcmFuc2Zvcm1DYWNoZSA9IG5ldyBXZWFrTWFwKCk7XG5mdW5jdGlvbiBwcm9taXNpZnlSZXF1ZXN0KHJlcXVlc3QpIHtcbiAgICBjb25zdCBwcm9taXNlID0gbmV3IFByb21pc2UoKHJlc29sdmUsIHJlamVjdCkgPT4ge1xuICAgICAgICBjb25zdCB1bmxpc3RlbiA9ICgpID0+IHtcbiAgICAgICAgICAgIHJlcXVlc3QucmVtb3ZlRXZlbnRMaXN0ZW5lcignc3VjY2VzcycsIHN1Y2Nlc3MpO1xuICAgICAgICAgICAgcmVxdWVzdC5yZW1vdmVFdmVudExpc3RlbmVyKCdlcnJvcicsIGVycm9yKTtcbiAgICAgICAgfTtcbiAgICAgICAgY29uc3Qgc3VjY2VzcyA9ICgpID0+IHtcbiAgICAgICAgICAgIHJlc29sdmUod3JhcChyZXF1ZXN0LnJlc3VsdCkpO1xuICAgICAgICAgICAgdW5saXN0ZW4oKTtcbiAgICAgICAgfTtcbiAgICAgICAgY29uc3QgZXJyb3IgPSAoKSA9PiB7XG4gICAgICAgICAgICByZWplY3QocmVxdWVzdC5lcnJvcik7XG4gICAgICAgICAgICB1bmxpc3RlbigpO1xuICAgICAgICB9O1xuICAgICAgICByZXF1ZXN0LmFkZEV2ZW50TGlzdGVuZXIoJ3N1Y2Nlc3MnLCBzdWNjZXNzKTtcbiAgICAgICAgcmVxdWVzdC5hZGRFdmVudExpc3RlbmVyKCdlcnJvcicsIGVycm9yKTtcbiAgICB9KTtcbiAgICBwcm9taXNlXG4gICAgICAgIC50aGVuKCh2YWx1ZSkgPT4ge1xuICAgICAgICAvLyBTaW5jZSBjdXJzb3JpbmcgcmV1c2VzIHRoZSBJREJSZXF1ZXN0ICgqc2lnaCopLCB3ZSBjYWNoZSBpdCBmb3IgbGF0ZXIgcmV0cmlldmFsXG4gICAgICAgIC8vIChzZWUgd3JhcEZ1bmN0aW9uKS5cbiAgICAgICAgaWYgKHZhbHVlIGluc3RhbmNlb2YgSURCQ3Vyc29yKSB7XG4gICAgICAgICAgICBjdXJzb3JSZXF1ZXN0TWFwLnNldCh2YWx1ZSwgcmVxdWVzdCk7XG4gICAgICAgIH1cbiAgICAgICAgLy8gQ2F0Y2hpbmcgdG8gYXZvaWQgXCJVbmNhdWdodCBQcm9taXNlIGV4Y2VwdGlvbnNcIlxuICAgIH0pXG4gICAgICAgIC5jYXRjaCgoKSA9PiB7IH0pO1xuICAgIC8vIFRoaXMgbWFwcGluZyBleGlzdHMgaW4gcmV2ZXJzZVRyYW5zZm9ybUNhY2hlIGJ1dCBkb2Vzbid0IGRvZXNuJ3QgZXhpc3QgaW4gdHJhbnNmb3JtQ2FjaGUuIFRoaXNcbiAgICAvLyBpcyBiZWNhdXNlIHdlIGNyZWF0ZSBtYW55IHByb21pc2VzIGZyb20gYSBzaW5nbGUgSURCUmVxdWVzdC5cbiAgICByZXZlcnNlVHJhbnNmb3JtQ2FjaGUuc2V0KHByb21pc2UsIHJlcXVlc3QpO1xuICAgIHJldHVybiBwcm9taXNlO1xufVxuZnVuY3Rpb24gY2FjaGVEb25lUHJvbWlzZUZvclRyYW5zYWN0aW9uKHR4KSB7XG4gICAgLy8gRWFybHkgYmFpbCBpZiB3ZSd2ZSBhbHJlYWR5IGNyZWF0ZWQgYSBkb25lIHByb21pc2UgZm9yIHRoaXMgdHJhbnNhY3Rpb24uXG4gICAgaWYgKHRyYW5zYWN0aW9uRG9uZU1hcC5oYXModHgpKVxuICAgICAgICByZXR1cm47XG4gICAgY29uc3QgZG9uZSA9IG5ldyBQcm9taXNlKChyZXNvbHZlLCByZWplY3QpID0+IHtcbiAgICAgICAgY29uc3QgdW5saXN0ZW4gPSAoKSA9PiB7XG4gICAgICAgICAgICB0eC5yZW1vdmVFdmVudExpc3RlbmVyKCdjb21wbGV0ZScsIGNvbXBsZXRlKTtcbiAgICAgICAgICAgIHR4LnJlbW92ZUV2ZW50TGlzdGVuZXIoJ2Vycm9yJywgZXJyb3IpO1xuICAgICAgICAgICAgdHgucmVtb3ZlRXZlbnRMaXN0ZW5lcignYWJvcnQnLCBlcnJvcik7XG4gICAgICAgIH07XG4gICAgICAgIGNvbnN0IGNvbXBsZXRlID0gKCkgPT4ge1xuICAgICAgICAgICAgcmVzb2x2ZSgpO1xuICAgICAgICAgICAgdW5saXN0ZW4oKTtcbiAgICAgICAgfTtcbiAgICAgICAgY29uc3QgZXJyb3IgPSAoKSA9PiB7XG4gICAgICAgICAgICByZWplY3QodHguZXJyb3IgfHwgbmV3IERPTUV4Y2VwdGlvbignQWJvcnRFcnJvcicsICdBYm9ydEVycm9yJykpO1xuICAgICAgICAgICAgdW5saXN0ZW4oKTtcbiAgICAgICAgfTtcbiAgICAgICAgdHguYWRkRXZlbnRMaXN0ZW5lcignY29tcGxldGUnLCBjb21wbGV0ZSk7XG4gICAgICAgIHR4LmFkZEV2ZW50TGlzdGVuZXIoJ2Vycm9yJywgZXJyb3IpO1xuICAgICAgICB0eC5hZGRFdmVudExpc3RlbmVyKCdhYm9ydCcsIGVycm9yKTtcbiAgICB9KTtcbiAgICAvLyBDYWNoZSBpdCBmb3IgbGF0ZXIgcmV0cmlldmFsLlxuICAgIHRyYW5zYWN0aW9uRG9uZU1hcC5zZXQodHgsIGRvbmUpO1xufVxubGV0IGlkYlByb3h5VHJhcHMgPSB7XG4gICAgZ2V0KHRhcmdldCwgcHJvcCwgcmVjZWl2ZXIpIHtcbiAgICAgICAgaWYgKHRhcmdldCBpbnN0YW5jZW9mIElEQlRyYW5zYWN0aW9uKSB7XG4gICAgICAgICAgICAvLyBTcGVjaWFsIGhhbmRsaW5nIGZvciB0cmFuc2FjdGlvbi5kb25lLlxuICAgICAgICAgICAgaWYgKHByb3AgPT09ICdkb25lJylcbiAgICAgICAgICAgICAgICByZXR1cm4gdHJhbnNhY3Rpb25Eb25lTWFwLmdldCh0YXJnZXQpO1xuICAgICAgICAgICAgLy8gUG9seWZpbGwgZm9yIG9iamVjdFN0b3JlTmFtZXMgYmVjYXVzZSBvZiBFZGdlLlxuICAgICAgICAgICAgaWYgKHByb3AgPT09ICdvYmplY3RTdG9yZU5hbWVzJykge1xuICAgICAgICAgICAgICAgIHJldHVybiB0YXJnZXQub2JqZWN0U3RvcmVOYW1lcyB8fCB0cmFuc2FjdGlvblN0b3JlTmFtZXNNYXAuZ2V0KHRhcmdldCk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICAvLyBNYWtlIHR4LnN0b3JlIHJldHVybiB0aGUgb25seSBzdG9yZSBpbiB0aGUgdHJhbnNhY3Rpb24sIG9yIHVuZGVmaW5lZCBpZiB0aGVyZSBhcmUgbWFueS5cbiAgICAgICAgICAgIGlmIChwcm9wID09PSAnc3RvcmUnKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuIHJlY2VpdmVyLm9iamVjdFN0b3JlTmFtZXNbMV1cbiAgICAgICAgICAgICAgICAgICAgPyB1bmRlZmluZWRcbiAgICAgICAgICAgICAgICAgICAgOiByZWNlaXZlci5vYmplY3RTdG9yZShyZWNlaXZlci5vYmplY3RTdG9yZU5hbWVzWzBdKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICAvLyBFbHNlIHRyYW5zZm9ybSB3aGF0ZXZlciB3ZSBnZXQgYmFjay5cbiAgICAgICAgcmV0dXJuIHdyYXAodGFyZ2V0W3Byb3BdKTtcbiAgICB9LFxuICAgIHNldCh0YXJnZXQsIHByb3AsIHZhbHVlKSB7XG4gICAgICAgIHRhcmdldFtwcm9wXSA9IHZhbHVlO1xuICAgICAgICByZXR1cm4gdHJ1ZTtcbiAgICB9LFxuICAgIGhhcyh0YXJnZXQsIHByb3ApIHtcbiAgICAgICAgaWYgKHRhcmdldCBpbnN0YW5jZW9mIElEQlRyYW5zYWN0aW9uICYmXG4gICAgICAgICAgICAocHJvcCA9PT0gJ2RvbmUnIHx8IHByb3AgPT09ICdzdG9yZScpKSB7XG4gICAgICAgICAgICByZXR1cm4gdHJ1ZTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gcHJvcCBpbiB0YXJnZXQ7XG4gICAgfSxcbn07XG5mdW5jdGlvbiByZXBsYWNlVHJhcHMoY2FsbGJhY2spIHtcbiAgICBpZGJQcm94eVRyYXBzID0gY2FsbGJhY2soaWRiUHJveHlUcmFwcyk7XG59XG5mdW5jdGlvbiB3cmFwRnVuY3Rpb24oZnVuYykge1xuICAgIC8vIER1ZSB0byBleHBlY3RlZCBvYmplY3QgZXF1YWxpdHkgKHdoaWNoIGlzIGVuZm9yY2VkIGJ5IHRoZSBjYWNoaW5nIGluIGB3cmFwYCksIHdlXG4gICAgLy8gb25seSBjcmVhdGUgb25lIG5ldyBmdW5jIHBlciBmdW5jLlxuICAgIC8vIEVkZ2UgZG9lc24ndCBzdXBwb3J0IG9iamVjdFN0b3JlTmFtZXMgKGJvb28pLCBzbyB3ZSBwb2x5ZmlsbCBpdCBoZXJlLlxuICAgIGlmIChmdW5jID09PSBJREJEYXRhYmFzZS5wcm90b3R5cGUudHJhbnNhY3Rpb24gJiZcbiAgICAgICAgISgnb2JqZWN0U3RvcmVOYW1lcycgaW4gSURCVHJhbnNhY3Rpb24ucHJvdG90eXBlKSkge1xuICAgICAgICByZXR1cm4gZnVuY3Rpb24gKHN0b3JlTmFtZXMsIC4uLmFyZ3MpIHtcbiAgICAgICAgICAgIGNvbnN0IHR4ID0gZnVuYy5jYWxsKHVud3JhcCh0aGlzKSwgc3RvcmVOYW1lcywgLi4uYXJncyk7XG4gICAgICAgICAgICB0cmFuc2FjdGlvblN0b3JlTmFtZXNNYXAuc2V0KHR4LCBzdG9yZU5hbWVzLnNvcnQgPyBzdG9yZU5hbWVzLnNvcnQoKSA6IFtzdG9yZU5hbWVzXSk7XG4gICAgICAgICAgICByZXR1cm4gd3JhcCh0eCk7XG4gICAgICAgIH07XG4gICAgfVxuICAgIC8vIEN1cnNvciBtZXRob2RzIGFyZSBzcGVjaWFsLCBhcyB0aGUgYmVoYXZpb3VyIGlzIGEgbGl0dGxlIG1vcmUgZGlmZmVyZW50IHRvIHN0YW5kYXJkIElEQi4gSW5cbiAgICAvLyBJREIsIHlvdSBhZHZhbmNlIHRoZSBjdXJzb3IgYW5kIHdhaXQgZm9yIGEgbmV3ICdzdWNjZXNzJyBvbiB0aGUgSURCUmVxdWVzdCB0aGF0IGdhdmUgeW91IHRoZVxuICAgIC8vIGN1cnNvci4gSXQncyBraW5kYSBsaWtlIGEgcHJvbWlzZSB0aGF0IGNhbiByZXNvbHZlIHdpdGggbWFueSB2YWx1ZXMuIFRoYXQgZG9lc24ndCBtYWtlIHNlbnNlXG4gICAgLy8gd2l0aCByZWFsIHByb21pc2VzLCBzbyBlYWNoIGFkdmFuY2UgbWV0aG9kcyByZXR1cm5zIGEgbmV3IHByb21pc2UgZm9yIHRoZSBjdXJzb3Igb2JqZWN0LCBvclxuICAgIC8vIHVuZGVmaW5lZCBpZiB0aGUgZW5kIG9mIHRoZSBjdXJzb3IgaGFzIGJlZW4gcmVhY2hlZC5cbiAgICBpZiAoZ2V0Q3Vyc29yQWR2YW5jZU1ldGhvZHMoKS5pbmNsdWRlcyhmdW5jKSkge1xuICAgICAgICByZXR1cm4gZnVuY3Rpb24gKC4uLmFyZ3MpIHtcbiAgICAgICAgICAgIC8vIENhbGxpbmcgdGhlIG9yaWdpbmFsIGZ1bmN0aW9uIHdpdGggdGhlIHByb3h5IGFzICd0aGlzJyBjYXVzZXMgSUxMRUdBTCBJTlZPQ0FUSU9OLCBzbyB3ZSB1c2VcbiAgICAgICAgICAgIC8vIHRoZSBvcmlnaW5hbCBvYmplY3QuXG4gICAgICAgICAgICBmdW5jLmFwcGx5KHVud3JhcCh0aGlzKSwgYXJncyk7XG4gICAgICAgICAgICByZXR1cm4gd3JhcChjdXJzb3JSZXF1ZXN0TWFwLmdldCh0aGlzKSk7XG4gICAgICAgIH07XG4gICAgfVxuICAgIHJldHVybiBmdW5jdGlvbiAoLi4uYXJncykge1xuICAgICAgICAvLyBDYWxsaW5nIHRoZSBvcmlnaW5hbCBmdW5jdGlvbiB3aXRoIHRoZSBwcm94eSBhcyAndGhpcycgY2F1c2VzIElMTEVHQUwgSU5WT0NBVElPTiwgc28gd2UgdXNlXG4gICAgICAgIC8vIHRoZSBvcmlnaW5hbCBvYmplY3QuXG4gICAgICAgIHJldHVybiB3cmFwKGZ1bmMuYXBwbHkodW53cmFwKHRoaXMpLCBhcmdzKSk7XG4gICAgfTtcbn1cbmZ1bmN0aW9uIHRyYW5zZm9ybUNhY2hhYmxlVmFsdWUodmFsdWUpIHtcbiAgICBpZiAodHlwZW9mIHZhbHVlID09PSAnZnVuY3Rpb24nKVxuICAgICAgICByZXR1cm4gd3JhcEZ1bmN0aW9uKHZhbHVlKTtcbiAgICAvLyBUaGlzIGRvZXNuJ3QgcmV0dXJuLCBpdCBqdXN0IGNyZWF0ZXMgYSAnZG9uZScgcHJvbWlzZSBmb3IgdGhlIHRyYW5zYWN0aW9uLFxuICAgIC8vIHdoaWNoIGlzIGxhdGVyIHJldHVybmVkIGZvciB0cmFuc2FjdGlvbi5kb25lIChzZWUgaWRiT2JqZWN0SGFuZGxlcikuXG4gICAgaWYgKHZhbHVlIGluc3RhbmNlb2YgSURCVHJhbnNhY3Rpb24pXG4gICAgICAgIGNhY2hlRG9uZVByb21pc2VGb3JUcmFuc2FjdGlvbih2YWx1ZSk7XG4gICAgaWYgKGluc3RhbmNlT2ZBbnkodmFsdWUsIGdldElkYlByb3h5YWJsZVR5cGVzKCkpKVxuICAgICAgICByZXR1cm4gbmV3IFByb3h5KHZhbHVlLCBpZGJQcm94eVRyYXBzKTtcbiAgICAvLyBSZXR1cm4gdGhlIHNhbWUgdmFsdWUgYmFjayBpZiB3ZSdyZSBub3QgZ29pbmcgdG8gdHJhbnNmb3JtIGl0LlxuICAgIHJldHVybiB2YWx1ZTtcbn1cbmZ1bmN0aW9uIHdyYXAodmFsdWUpIHtcbiAgICAvLyBXZSBzb21ldGltZXMgZ2VuZXJhdGUgbXVsdGlwbGUgcHJvbWlzZXMgZnJvbSBhIHNpbmdsZSBJREJSZXF1ZXN0IChlZyB3aGVuIGN1cnNvcmluZyksIGJlY2F1c2VcbiAgICAvLyBJREIgaXMgd2VpcmQgYW5kIGEgc2luZ2xlIElEQlJlcXVlc3QgY2FuIHlpZWxkIG1hbnkgcmVzcG9uc2VzLCBzbyB0aGVzZSBjYW4ndCBiZSBjYWNoZWQuXG4gICAgaWYgKHZhbHVlIGluc3RhbmNlb2YgSURCUmVxdWVzdClcbiAgICAgICAgcmV0dXJuIHByb21pc2lmeVJlcXVlc3QodmFsdWUpO1xuICAgIC8vIElmIHdlJ3ZlIGFscmVhZHkgdHJhbnNmb3JtZWQgdGhpcyB2YWx1ZSBiZWZvcmUsIHJldXNlIHRoZSB0cmFuc2Zvcm1lZCB2YWx1ZS5cbiAgICAvLyBUaGlzIGlzIGZhc3RlciwgYnV0IGl0IGFsc28gcHJvdmlkZXMgb2JqZWN0IGVxdWFsaXR5LlxuICAgIGlmICh0cmFuc2Zvcm1DYWNoZS5oYXModmFsdWUpKVxuICAgICAgICByZXR1cm4gdHJhbnNmb3JtQ2FjaGUuZ2V0KHZhbHVlKTtcbiAgICBjb25zdCBuZXdWYWx1ZSA9IHRyYW5zZm9ybUNhY2hhYmxlVmFsdWUodmFsdWUpO1xuICAgIC8vIE5vdCBhbGwgdHlwZXMgYXJlIHRyYW5zZm9ybWVkLlxuICAgIC8vIFRoZXNlIG1heSBiZSBwcmltaXRpdmUgdHlwZXMsIHNvIHRoZXkgY2FuJ3QgYmUgV2Vha01hcCBrZXlzLlxuICAgIGlmIChuZXdWYWx1ZSAhPT0gdmFsdWUpIHtcbiAgICAgICAgdHJhbnNmb3JtQ2FjaGUuc2V0KHZhbHVlLCBuZXdWYWx1ZSk7XG4gICAgICAgIHJldmVyc2VUcmFuc2Zvcm1DYWNoZS5zZXQobmV3VmFsdWUsIHZhbHVlKTtcbiAgICB9XG4gICAgcmV0dXJuIG5ld1ZhbHVlO1xufVxuY29uc3QgdW53cmFwID0gKHZhbHVlKSA9PiByZXZlcnNlVHJhbnNmb3JtQ2FjaGUuZ2V0KHZhbHVlKTtcblxuZXhwb3J0IHsgcmV2ZXJzZVRyYW5zZm9ybUNhY2hlIGFzIGEsIGluc3RhbmNlT2ZBbnkgYXMgaSwgcmVwbGFjZVRyYXBzIGFzIHIsIHVud3JhcCBhcyB1LCB3cmFwIGFzIHcgfTtcbiIsImltcG9ydCB7IEluZGV4ZWREYk1hbmFnZXIgfSBmcm9tICcuL2luZGV4ZWREYkJsYXpvcic7XG5cbm5hbWVzcGFjZSBJbmRleERiIHtcbiAgICBjb25zdCB0aW1lZ2hvc3RFeHRlbnNpb25zOiBzdHJpbmcgPSAnQmxhem9ySW5kZXhlZERiSnMnO1xuICAgIGNvbnN0IGV4dGVuc2lvbk9iamVjdCA9IHtcbiAgICAgICAgSURCTWFuYWdlcjogbmV3IEluZGV4ZWREYk1hbmFnZXIoKVxuICAgIH07XG5cbiAgICBleHBvcnQgZnVuY3Rpb24gaW5pdGlhbGlzZSgpOiB2b2lkIHtcbiAgICAgICAgaWYgKHR5cGVvZiB3aW5kb3cgIT09ICd1bmRlZmluZWQnICYmICF3aW5kb3dbdGltZWdob3N0RXh0ZW5zaW9uc10pIHtcbiAgICAgICAgICAgIHdpbmRvd1t0aW1lZ2hvc3RFeHRlbnNpb25zXSA9IHtcbiAgICAgICAgICAgICAgICAuLi5leHRlbnNpb25PYmplY3RcbiAgICAgICAgICAgIH07XG4gICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICB3aW5kb3dbdGltZWdob3N0RXh0ZW5zaW9uc10gPSB7XG4gICAgICAgICAgICAgICAgLi4ud2luZG93W3RpbWVnaG9zdEV4dGVuc2lvbnNdLFxuICAgICAgICAgICAgICAgIC4uLmV4dGVuc2lvbk9iamVjdFxuICAgICAgICAgICAgfTtcbiAgICAgICAgfVxuXG4gICAgfVxufVxuXG5JbmRleERiLmluaXRpYWxpc2UoKTsiLCIvLy8vLyA8cmVmZXJlbmNlIHBhdGg9XCJNaWNyb3NvZnQuSlNJbnRlcm9wLmQudHNcIi8+XG5pbXBvcnQgeyBvcGVuREIsIGRlbGV0ZURCLCBJREJQRGF0YWJhc2UsIElEQlBPYmplY3RTdG9yZSB9IGZyb20gJ2lkYic7XG5pbXBvcnQgeyBJRGF0YWJhc2UsIElPYmplY3RTdG9yZSwgSUluZGV4IH0gZnJvbSAnLi9JbnRlcm9wSW50ZXJmYWNlcyc7XG5cbmNvbnN0IEVfREJfQ0xPU0VEOiBzdHJpbmcgPSBcIkRhdGFiYXNlIGlzIGNsb3NlZFwiO1xuXG5leHBvcnQgY2xhc3MgSW5kZXhlZERiTWFuYWdlciB7XG5cbiAgICBwcml2YXRlIGRiSW5zdGFuY2U/OiBJREJQRGF0YWJhc2UgPSB1bmRlZmluZWQ7XG5cbiAgICBjb25zdHJ1Y3RvcigpIHsgfVxuXG4gICAgcHVibGljIG9wZW4gPSBhc3luYyAoZGF0YWJhc2U6IElEYXRhYmFzZSk6IFByb21pc2U8dm9pZD4gPT4ge1xuICAgICAgICB2YXIgdXBncmFkZUVycm9yID0gXCJcIjtcblxuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UgfHwgdGhpcy5kYkluc3RhbmNlLnZlcnNpb24gPCBkYXRhYmFzZS52ZXJzaW9uKSB7XG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuZGJJbnN0YW5jZSkge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmRiSW5zdGFuY2UuY2xvc2UoKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5kYkluc3RhbmNlID0gdW5kZWZpbmVkO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB0aGlzLmRiSW5zdGFuY2UgPSBhd2FpdCBvcGVuREIoZGF0YWJhc2UubmFtZSwgZGF0YWJhc2UudmVyc2lvbiwge1xuICAgICAgICAgICAgICAgICAgICB1cGdyYWRlKGRiLCBvbGRWZXJzaW9uLCBuZXdWZXJzaW9uLCB0cmFuc2FjdGlvbikge1xuICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBJbmRleGVkRGJNYW5hZ2VyLnVwZ3JhZGVEYXRhYmFzZShkYiwgb2xkVmVyc2lvbiwgbmV3VmVyc2lvbiwgZGF0YWJhc2UpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB1cGdyYWRlRXJyb3IgPSBlcnJvci50b1N0cmluZygpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRocm93KGVycm9yKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGVycm9yLnRvU3RyaW5nKCkrJyAnK3VwZ3JhZGVFcnJvcjtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBkZWxldGVEYXRhYmFzZSA9IGFzeW5jKGRiTmFtZTogc3RyaW5nKTogUHJvbWlzZTx2b2lkPiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIHRoaXMuZGJJbnN0YW5jZS5jbG9zZSgpO1xuXG4gICAgICAgICAgICBhd2FpdCBkZWxldGVEQihkYk5hbWUpO1xuXG4gICAgICAgICAgICB0aGlzLmRiSW5zdGFuY2UgPSB1bmRlZmluZWQ7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgRGF0YWJhc2UgJHtkYk5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXREYlNjaGVtYSA9IGFzeW5jIChkYk5hbWU6IHN0cmluZykgOiBQcm9taXNlPElEYXRhYmFzZT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCBkYkluc3RhbmNlID0gdGhpcy5kYkluc3RhbmNlO1xuXG4gICAgICAgICAgICBjb25zdCBkYkluZm86IElEYXRhYmFzZSA9IHtcbiAgICAgICAgICAgICAgICBuYW1lOiBkYkluc3RhbmNlLm5hbWUsXG4gICAgICAgICAgICAgICAgdmVyc2lvbjogZGJJbnN0YW5jZS52ZXJzaW9uLFxuICAgICAgICAgICAgICAgIG9iamVjdFN0b3JlczogW11cbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgZm9yIChsZXQgcyA9IDA7IHMgPCBkYkluc3RhbmNlLm9iamVjdFN0b3JlTmFtZXMubGVuZ3RoOyBzKyspIHtcbiAgICAgICAgICAgICAgICBsZXQgZGJTdG9yZSA9IGRiSW5zdGFuY2UudHJhbnNhY3Rpb24oZGJJbnN0YW5jZS5vYmplY3RTdG9yZU5hbWVzW3NdLCAncmVhZG9ubHknKS5zdG9yZTtcbiAgICAgICAgICAgICAgICBsZXQgb2JqZWN0U3RvcmU6IElPYmplY3RTdG9yZSA9IHtcbiAgICAgICAgICAgICAgICAgICAgbmFtZTogZGJTdG9yZS5uYW1lLFxuICAgICAgICAgICAgICAgICAgICBrZXlQYXRoOiBBcnJheS5pc0FycmF5KGRiU3RvcmUua2V5UGF0aCkgPyBkYlN0b3JlLmtleVBhdGguam9pbignLCcpIDogZGJTdG9yZS5rZXlQYXRoLFxuICAgICAgICAgICAgICAgICAgICBhdXRvSW5jcmVtZW50OiBkYlN0b3JlLmF1dG9JbmNyZW1lbnQsXG4gICAgICAgICAgICAgICAgICAgIGluZGV4ZXM6IFtdXG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgZGJTdG9yZS5pbmRleE5hbWVzLmxlbmd0aDsgaSsrKSB7XG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGRiSW5kZXggPSBkYlN0b3JlLmluZGV4KGRiU3RvcmUuaW5kZXhOYW1lc1tpXSk7XG4gICAgICAgICAgICAgICAgICAgIGxldCBpbmRleDogSUluZGV4ID0ge1xuICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogZGJJbmRleC5uYW1lLFxuICAgICAgICAgICAgICAgICAgICAgICAga2V5UGF0aDogQXJyYXkuaXNBcnJheShkYkluZGV4LmtleVBhdGgpID8gZGJJbmRleC5rZXlQYXRoLmpvaW4oJywnKSA6IGRiSW5kZXgua2V5UGF0aCxcbiAgICAgICAgICAgICAgICAgICAgICAgIG11bHRpRW50cnk6IGRiSW5kZXgubXVsdGlFbnRyeSxcbiAgICAgICAgICAgICAgICAgICAgICAgIHVuaXF1ZTogZGJJbmRleC51bmlxdWVcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICBvYmplY3RTdG9yZS5pbmRleGVzLnB1c2goaW5kZXgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBkYkluZm8ub2JqZWN0U3RvcmVzLnB1c2gob2JqZWN0U3RvcmUpO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICByZXR1cm4gZGJJbmZvO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYERhdGFiYXNlICR7ZGJOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICAvLyBJREJPYmplY3RTdG9yZVxuICAgIHB1YmxpYyBjb3VudCA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywga2V5PzogYW55KTogUHJvbWlzZTxudW1iZXI+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZG9ubHknKTtcblxuICAgICAgICAgICAgbGV0IHJlc3VsdCA9IGF3YWl0IHR4LnN0b3JlLmNvdW50KGtleSA/PyB1bmRlZmluZWQpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0O1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgY291bnRCeUtleVJhbmdlID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBsb3dlcjogYW55LCB1cHBlcjogYW55LCBsb3dlck9wZW46IGJvb2xlYW4sIHVwcGVyT3BlbjogYm9vbGVhbik6IFByb21pc2U8bnVtYmVyPiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICByZXR1cm4gYXdhaXQgdGhpcy5jb3VudChzdG9yZU5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pKTtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldCA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywga2V5OiBhbnkpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0ID0gYXdhaXQgdHguc3RvcmUuZ2V0KGtleSk7XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHQ7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRBbGwgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGtleT86IGFueSwgY291bnQ/OiBudW1iZXIpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0cyA9IGF3YWl0IHR4LnN0b3JlLmdldEFsbChrZXkgPz8gdW5kZWZpbmVkLCBjb3VudCA/PyB1bmRlZmluZWQpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0cztcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEJ5S2V5UmFuZ2UgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGxvd2VyOiBhbnksIHVwcGVyOiBhbnksIGxvd2VyT3BlbjogYm9vbGVhbiwgdXBwZXJPcGVuOiBib29sZWFuLCBjb3VudD86IG51bWJlcik6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmdldEFsbChzdG9yZU5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pLCBjb3VudCk7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRBbGxCeUFycmF5S2V5ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBrZXk6IGFueVtdKTogUHJvbWlzZTxhbnk+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZG9ubHknKTtcblxuICAgICAgICAgICAgbGV0IHJlc3VsdHM6IGFueVtdID0gW107XG5cbiAgICAgICAgICAgIGZvciAobGV0IGluZGV4ID0gMDsgaW5kZXggPCBrZXkubGVuZ3RoOyBpbmRleCsrKSB7XG4gICAgICAgICAgICAgICAgY29uc3QgZWxlbWVudCA9IGtleVtpbmRleF07XG4gICAgICAgICAgICAgICAgcmVzdWx0cyA9IHJlc3VsdHMuY29uY2F0KGF3YWl0IHR4LnN0b3JlLmdldEFsbChlbGVtZW50KSk7XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHRzO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgZ2V0S2V5ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBrZXk6IGFueSk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWRvbmx5Jyk7XG5cbiAgICAgICAgICAgIGxldCByZXN1bHQgPSBhd2FpdCB0eC5zdG9yZS5nZXRLZXkoa2V5KTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEtleXMgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGtleT86IGFueSwgY291bnQ/OiBudW1iZXIpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0cyA9IGF3YWl0IHR4LnN0b3JlLmdldEFsbEtleXMoa2V5ID8/IHVuZGVmaW5lZCwgY291bnQgPz8gdW5kZWZpbmVkKTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdHM7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRBbGxLZXlzQnlLZXlSYW5nZSA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgbG93ZXI6IGFueSwgdXBwZXI6IGFueSwgbG93ZXJPcGVuOiBib29sZWFuLCB1cHBlck9wZW46IGJvb2xlYW4sIGNvdW50PzogbnVtYmVyKTogUHJvbWlzZTxhbnk+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgcmV0dXJuIGF3YWl0IHRoaXMuZ2V0QWxsS2V5cyhzdG9yZU5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pLCBjb3VudCk7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRBbGxLZXlzQnlBcnJheUtleSA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywga2V5OiBhbnlbXSk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWRvbmx5Jyk7XG5cbiAgICAgICAgICAgIGxldCByZXN1bHRzOiBhbnlbXSA9IFtdO1xuXG4gICAgICAgICAgICBmb3IgKGxldCBpbmRleCA9IDA7IGluZGV4IDwga2V5Lmxlbmd0aDsgaW5kZXgrKykge1xuICAgICAgICAgICAgICAgIGNvbnN0IGVsZW1lbnQgPSBrZXlbaW5kZXhdO1xuICAgICAgICAgICAgICAgIHJlc3VsdHMgPSByZXN1bHRzLmNvbmNhdChhd2FpdCB0eC5zdG9yZS5nZXRBbGxLZXlzKGVsZW1lbnQpKTtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdHM7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBxdWVyeSA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywga2V5OiBhbnksIGZpbHRlcjogc3RyaW5nLCBjb3VudDogbnVtYmVyID0gMCwgc2tpcDogbnVtYmVyID0gMCk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgdmFyIGZ1bmMgPSBuZXcgRnVuY3Rpb24oJ29iaicsIGZpbHRlcik7XG4gICAgICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgICAgIHRocm93IGAke2Vycm9yLnRvU3RyaW5nKCl9IGluIGZpbHRlciB7ICR7ZmlsdGVyfSB9YFxuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICB2YXIgcm93ID0gMDtcbiAgICAgICAgICAgIHZhciBlcnJvck1lc3NhZ2UgPSBcIlwiO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0czogYW55W10gPSBbXTtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZG9ubHknKTtcblxuICAgICAgICAgICAgbGV0IGN1cnNvciA9IGF3YWl0IHR4LnN0b3JlLm9wZW5DdXJzb3Ioa2V5ID8/IHVuZGVmaW5lZCk7XG4gICAgICAgICAgICB3aGlsZSAoY3Vyc29yKSB7XG4gICAgICAgICAgICAgICAgaWYgKCFjdXJzb3IpIHtcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB0cnkge1xuICAgICAgICAgICAgICAgICAgICB2YXIgb3V0ID0gZnVuYyhjdXJzb3IudmFsdWUpO1xuICAgICAgICAgICAgICAgICAgICBpZiAob3V0KSB7XG4gICAgICAgICAgICAgICAgICAgICAgICByb3cgKys7XG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocm93ID4gc2tpcCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlc3VsdHMucHVzaChvdXQpO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgICAgICAgICBlcnJvck1lc3NhZ2UgPSBgb2JqOiAke0pTT04uc3RyaW5naWZ5KGN1cnNvci52YWx1ZSl9XFxuZmlsdGVyOiAke2ZpbHRlcn1cXG5lcnJvcjogJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgaWYgKGNvdW50ID4gMCAmJiByZXN1bHRzLmxlbmd0aCA+PSBjb3VudCkge1xuICAgICAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGN1cnNvciA9IGF3YWl0IGN1cnNvci5jb250aW51ZSgpO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuXG4gICAgICAgICAgICBpZiAoZXJyb3JNZXNzYWdlKSB7XG4gICAgICAgICAgICAgICAgdGhyb3cgZXJyb3JNZXNzYWdlO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0cztcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0gJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICAvLyBJREJJbmRleCBmdW5jdGlvbnNcbiAgICBwdWJsaWMgY291bnRGcm9tSW5kZXggPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGluZGV4TmFtZTogc3RyaW5nLCBrZXk/OiBhbnkpOiBQcm9taXNlPG51bWJlcj4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0ID0gYXdhaXQgdHguc3RvcmUuaW5kZXgoaW5kZXhOYW1lKS5jb3VudChrZXkgPz8gdW5kZWZpbmVkKTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sIEluZGV4ICR7aW5kZXhOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgY291bnRGcm9tSW5kZXhCeUtleVJhbmdlID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBpbmRleE5hbWU6IHN0cmluZywgbG93ZXI6IGFueSwgdXBwZXI6IGFueSwgbG93ZXJPcGVuOiBib29sZWFuLCB1cHBlck9wZW46IGJvb2xlYW4pOiBQcm9taXNlPG51bWJlcj4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgcmV0dXJuIGF3YWl0IHRoaXMuY291bnRGcm9tSW5kZXgoc3RvcmVOYW1lLCBpbmRleE5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pKTtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sIEluZGV4ICR7aW5kZXhOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgZ2V0RnJvbUluZGV4ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBpbmRleE5hbWU6IHN0cmluZywga2V5OiBhbnkpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBjb25zdCByZXN1bHRzID0gYXdhaXQgdHguc3RvcmUuaW5kZXgoaW5kZXhOYW1lKS5nZXQoa2V5KTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdHM7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCBJbmRleCAke2luZGV4TmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEZyb21JbmRleCA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgaW5kZXhOYW1lOiBzdHJpbmcsIGtleT86IGFueSwgY291bnQ/OiBudW1iZXIpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBjb25zdCByZXN1bHRzID0gYXdhaXQgdHguc3RvcmUuaW5kZXgoaW5kZXhOYW1lKS5nZXRBbGwoa2V5ID8/IHVuZGVmaW5lZCwgY291bnQgPz8gdW5kZWZpbmVkKTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdHM7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCBJbmRleCAke2luZGV4TmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEZyb21JbmRleEJ5S2V5UmFuZ2UgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGluZGV4TmFtZTogc3RyaW5nLCBsb3dlcjogYW55LCB1cHBlcjogYW55LCBsb3dlck9wZW46IGJvb2xlYW4sIHVwcGVyT3BlbjogYm9vbGVhbiwgY291bnQ/OiBudW1iZXIpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICByZXR1cm4gYXdhaXQgdGhpcy5nZXRBbGxGcm9tSW5kZXgoc3RvcmVOYW1lLCBpbmRleE5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pLCBjb3VudCk7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCBJbmRleCAke2luZGV4TmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEZyb21JbmRleEJ5QXJyYXlLZXkgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGluZGV4TmFtZTogc3RyaW5nLCBrZXk6IGFueVtdKTogUHJvbWlzZTxhbnk+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZG9ubHknKTtcbiAgICAgICAgICAgIGNvbnN0IGR4ID0gdHguc3RvcmUuaW5kZXgoaW5kZXhOYW1lKTtcblxuICAgICAgICAgICAgbGV0IHJlc3VsdHM6IGFueVtdID0gW107XG5cbiAgICAgICAgICAgIGZvciAobGV0IGluZGV4ID0gMDsgaW5kZXggPCBrZXkubGVuZ3RoOyBpbmRleCsrKSB7XG4gICAgICAgICAgICAgICAgY29uc3QgZWxlbWVudCA9IGtleVtpbmRleF07XG4gICAgICAgICAgICAgICAgcmVzdWx0cyA9IHJlc3VsdHMuY29uY2F0KGF3YWl0IGR4LmdldEFsbChlbGVtZW50KSk7XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHRzO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgSW5kZXggJHtpbmRleE5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRLZXlGcm9tSW5kZXggPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGluZGV4TmFtZTogc3RyaW5nLCBrZXk6IGFueSk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWRvbmx5Jyk7XG5cbiAgICAgICAgICAgIGNvbnN0IHJlc3VsdHMgPSBhd2FpdCB0eC5zdG9yZS5pbmRleChpbmRleE5hbWUpLmdldEtleShrZXkpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0cztcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sIEluZGV4ICR7aW5kZXhOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgZ2V0QWxsS2V5c0Zyb21JbmRleCA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgaW5kZXhOYW1lOiBzdHJpbmcsIGtleT86IGFueSwgY291bnQ/OiBudW1iZXIpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkb25seScpO1xuXG4gICAgICAgICAgICBjb25zdCByZXN1bHRzID0gYXdhaXQgdHguc3RvcmUuaW5kZXgoaW5kZXhOYW1lKS5nZXRBbGxLZXlzKGtleSA/PyB1bmRlZmluZWQsIGNvdW50ID8/IHVuZGVmaW5lZCk7XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHRzO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgSW5kZXggJHtpbmRleE5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBnZXRBbGxLZXlzRnJvbUluZGV4QnlLZXlSYW5nZSA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgaW5kZXhOYW1lOiBzdHJpbmcsIGxvd2VyOiBhbnksIHVwcGVyOiBhbnksIGxvd2VyT3BlbjogYm9vbGVhbiwgdXBwZXJPcGVuOiBib29sZWFuLCBjb3VudD86IG51bWJlcik6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIHJldHVybiBhd2FpdCB0aGlzLmdldEFsbEtleXNGcm9tSW5kZXgoc3RvcmVOYW1lLCBpbmRleE5hbWUsIElEQktleVJhbmdlLmJvdW5kKGxvd2VyLCB1cHBlciwgbG93ZXJPcGVuLCB1cHBlck9wZW4pLCBjb3VudCk7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCBJbmRleCAke2luZGV4TmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGdldEFsbEtleXNGcm9tSW5kZXhCeUFycmF5S2V5ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBpbmRleE5hbWU6IHN0cmluZywga2V5OiBhbnlbXSk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWRvbmx5Jyk7XG4gICAgICAgICAgICBjb25zdCBkeCA9IHR4LnN0b3JlLmluZGV4KGluZGV4TmFtZSk7XG5cbiAgICAgICAgICAgIGxldCByZXN1bHRzOiBhbnlbXSA9IFtdO1xuXG4gICAgICAgICAgICBmb3IgKGxldCBpbmRleCA9IDA7IGluZGV4IDwga2V5Lmxlbmd0aDsgaW5kZXgrKykge1xuICAgICAgICAgICAgICAgIGNvbnN0IGVsZW1lbnQgPSBrZXlbaW5kZXhdO1xuICAgICAgICAgICAgICAgIHJlc3VsdHMgPSByZXN1bHRzLmNvbmNhdChhd2FpdCBkeC5nZXRBbGxLZXlzKGVsZW1lbnQpKTtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdHM7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCBJbmRleCAke2luZGV4TmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIHF1ZXJ5RnJvbUluZGV4ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBpbmRleE5hbWU6IHN0cmluZywga2V5OiBhbnksIGZpbHRlcjogc3RyaW5nLCBjb3VudDogbnVtYmVyID0gMCwgc2tpcDogbnVtYmVyID0gMCk6IFByb21pc2U8YW55PiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgdmFyIGZ1bmMgPSBuZXcgRnVuY3Rpb24oJ29iaicsIGZpbHRlcik7XG4gICAgICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgICAgIHRocm93IGAke2Vycm9yLnRvU3RyaW5nKCl9IGluIGZpbHRlciB7ICR7ZmlsdGVyfSB9YFxuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICB2YXIgcm93ID0gMDtcbiAgICAgICAgICAgIHZhciBlcnJvck1lc3NhZ2UgPSBcIlwiO1xuXG4gICAgICAgICAgICBsZXQgcmVzdWx0czogYW55W10gPSBbXTtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZG9ubHknKTtcblxuICAgICAgICAgICAgbGV0IGN1cnNvciA9IGF3YWl0IHR4LnN0b3JlLmluZGV4KGluZGV4TmFtZSkub3BlbkN1cnNvcihrZXkgPz8gdW5kZWZpbmVkKTtcbiAgICAgICAgICAgIHdoaWxlIChjdXJzb3IpIHtcbiAgICAgICAgICAgICAgICBpZiAoIWN1cnNvcikge1xuICAgICAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgICAgIHZhciBvdXQgPSBmdW5jKGN1cnNvci52YWx1ZSk7XG4gICAgICAgICAgICAgICAgICAgIGlmIChvdXQpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvdyArKztcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPiBza2lwKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVzdWx0cy5wdXNoKG91dCk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICAgICAgICAgIGVycm9yTWVzc2FnZSA9IGBvYmo6ICR7SlNPTi5zdHJpbmdpZnkoY3Vyc29yLnZhbHVlKX1cXG5maWx0ZXI6ICR7ZmlsdGVyfVxcbmVycm9yOiAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBpZiAoY291bnQgPiAwICYmIHJlc3VsdHMubGVuZ3RoID49IGNvdW50KSB7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgY3Vyc29yID0gYXdhaXQgY3Vyc29yLmNvbnRpbnVlKCk7XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIGlmIChlcnJvck1lc3NhZ2UpIHtcbiAgICAgICAgICAgICAgICB0aHJvdyBlcnJvck1lc3NhZ2U7XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHRzO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgSW5kZXggJHtpbmRleE5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBhZGQgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGRhdGE6IGFueSwga2V5PzogYW55KTogUHJvbWlzZTxhbnk+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZHdyaXRlJyk7XG5cbiAgICAgICAgICAgIGRhdGEgPSB0aGlzLmNoZWNrRm9yS2V5UGF0aCh0eC5zdG9yZSwgZGF0YSk7XG5cbiAgICAgICAgICAgIGNvbnN0IHJlc3VsdCA9IGF3YWl0IHR4LnN0b3JlLmFkZChkYXRhLCBrZXkgPz8gdW5kZWZpbmVkKTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIHB1dCA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgZGF0YTogYW55LCBrZXk/OiBhbnkpOiBQcm9taXNlPGFueT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkd3JpdGUnKTtcblxuICAgICAgICAgICAgY29uc3QgcmVzdWx0ID0gYXdhaXQgdHguc3RvcmUucHV0KGRhdGEsIGtleSA/PyB1bmRlZmluZWQpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0O1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgZGVsZXRlID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBpZDogYW55KTogUHJvbWlzZTx2b2lkPiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWR3cml0ZScpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5zdG9yZS5kZWxldGUoaWQpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwdWJsaWMgYmF0Y2hBZGQgPSBhc3luYyAoc3RvcmVOYW1lOiBzdHJpbmcsIGRhdGE6IGFueVtdKTogUHJvbWlzZTxhbnlbXT4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkd3JpdGUnKTtcblxuICAgICAgICAgICAgbGV0IHJlc3VsdDogYW55W10gPSBbXTtcblxuICAgICAgICAgICAgZGF0YS5mb3JFYWNoKGFzeW5jIGVsZW1lbnQgPT4ge1xuICAgICAgICAgICAgICAgIGxldCBpdGVtID0gdGhpcy5jaGVja0ZvcktleVBhdGgodHguc3RvcmUsIGVsZW1lbnQpO1xuICAgICAgICAgICAgICAgIHJlc3VsdC5wdXNoKGF3YWl0IHR4LnN0b3JlLmFkZChpdGVtKSk7XG4gICAgICAgICAgICB9KTtcblxuICAgICAgICAgICAgYXdhaXQgdHguZG9uZTtcblxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdDtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRocm93IGBTdG9yZSAke3N0b3JlTmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgcHVibGljIGJhdGNoUHV0ID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nLCBkYXRhOiBhbnlbXSk6IFByb21pc2U8YW55W10+ID0+IHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGlmICghdGhpcy5kYkluc3RhbmNlKSB0aHJvdyBFX0RCX0NMT1NFRDtcblxuICAgICAgICAgICAgY29uc3QgdHggPSB0aGlzLmRiSW5zdGFuY2UudHJhbnNhY3Rpb24oc3RvcmVOYW1lLCAncmVhZHdyaXRlJyk7XG5cbiAgICAgICAgICAgIGxldCByZXN1bHQ6IGFueVtdID0gW107XG5cbiAgICAgICAgICAgIGRhdGEuZm9yRWFjaChhc3luYyBlbGVtZW50ID0+IHtcbiAgICAgICAgICAgICAgICByZXN1bHQucHVzaChhd2FpdCB0eC5zdG9yZS5wdXQoZWxlbWVudCkpO1xuICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG5cbiAgICAgICAgICAgIHJldHVybiByZXN1bHQ7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBiYXRjaERlbGV0ZSA9IGFzeW5jIChzdG9yZU5hbWU6IHN0cmluZywgaWRzOiBhbnlbXSk6IFByb21pc2U8dm9pZD4gPT4ge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgaWYgKCF0aGlzLmRiSW5zdGFuY2UpIHRocm93IEVfREJfQ0xPU0VEO1xuXG4gICAgICAgICAgICBjb25zdCB0eCA9IHRoaXMuZGJJbnN0YW5jZS50cmFuc2FjdGlvbihzdG9yZU5hbWUsICdyZWFkd3JpdGUnKTtcblxuICAgICAgICAgICAgaWRzLmZvckVhY2goYXN5bmMgZWxlbWVudCA9PiB7XG4gICAgICAgICAgICAgICAgYXdhaXQgdHguc3RvcmUuZGVsZXRlKGVsZW1lbnQpO1xuICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgIGF3YWl0IHR4LmRvbmU7XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICB0aHJvdyBgU3RvcmUgJHtzdG9yZU5hbWV9LCAke2Vycm9yLnRvU3RyaW5nKCl9YDtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBjbGVhclN0b3JlID0gYXN5bmMgKHN0b3JlTmFtZTogc3RyaW5nKTogUHJvbWlzZTx2b2lkPiA9PiB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBpZiAoIXRoaXMuZGJJbnN0YW5jZSkgdGhyb3cgRV9EQl9DTE9TRUQ7XG5cbiAgICAgICAgICAgIGNvbnN0IHR4ID0gdGhpcy5kYkluc3RhbmNlLnRyYW5zYWN0aW9uKHN0b3JlTmFtZSwgJ3JlYWR3cml0ZScpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5zdG9yZS5jbGVhcigpO1xuXG4gICAgICAgICAgICBhd2FpdCB0eC5kb25lO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYFN0b3JlICR7c3RvcmVOYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwcml2YXRlIGNoZWNrRm9yS2V5UGF0aChvYmplY3RTdG9yZTogSURCUE9iamVjdFN0b3JlPGFueSwgYW55PiwgZGF0YTogYW55KSB7XG4gICAgICAgIGlmICghb2JqZWN0U3RvcmUuYXV0b0luY3JlbWVudCB8fCAhb2JqZWN0U3RvcmUua2V5UGF0aCkge1xuICAgICAgICAgICAgcmV0dXJuIGRhdGE7XG4gICAgICAgIH1cblxuICAgICAgICBpZiAodHlwZW9mIG9iamVjdFN0b3JlLmtleVBhdGggIT09ICdzdHJpbmcnKSB7XG4gICAgICAgICAgICByZXR1cm4gZGF0YTtcbiAgICAgICAgfVxuXG4gICAgICAgIGNvbnN0IGtleVBhdGggPSBvYmplY3RTdG9yZS5rZXlQYXRoIGFzIHN0cmluZztcblxuICAgICAgICBpZiAoIWRhdGFba2V5UGF0aF0pIHtcbiAgICAgICAgICAgIGRlbGV0ZSBkYXRhW2tleVBhdGhdO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBkYXRhO1xuICAgIH1cblxuICAgIHByaXZhdGUgc3RhdGljIHVwZ3JhZGVEYXRhYmFzZSh1cGdyYWRlREI6IElEQlBEYXRhYmFzZSwgb2xkVmVyc2lvbjogbnVtYmVyLCBuZXdWZXJzaW9uOiBudW1iZXIgfCBudWxsLCBkYkRhdGFiYXNlOiBJRGF0YWJhc2UpIHtcbiAgICAgICAgaWYgKG5ld1ZlcnNpb24gJiYgbmV3VmVyc2lvbiA+IG9sZFZlcnNpb24pIHtcbiAgICAgICAgICAgIGlmIChkYkRhdGFiYXNlLm9iamVjdFN0b3Jlcykge1xuICAgICAgICAgICAgICAgIGZvciAodmFyIHN0b3JlIG9mIGRiRGF0YWJhc2Uub2JqZWN0U3RvcmVzKSB7XG4gICAgICAgICAgICAgICAgICAgIGlmICghdXBncmFkZURCLm9iamVjdFN0b3JlTmFtZXMuY29udGFpbnMoc3RvcmUubmFtZSkpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuYWRkTmV3U3RvcmUodXBncmFkZURCLCBzdG9yZSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwcml2YXRlIHN0YXRpYyBnZXRLZXlQYXRoKGtleVBhdGg/OiBzdHJpbmcpOiBzdHJpbmcgfCBzdHJpbmdbXSB8IHVuZGVmaW5lZCB7XG4gICAgICAgIGlmIChrZXlQYXRoKSB7XG4gICAgICAgICAgICB2YXIgbXVsdGlLZXlQYXRoID0ga2V5UGF0aC5zcGxpdCgnLCcpO1xuICAgICAgICAgICAgcmV0dXJuIG11bHRpS2V5UGF0aC5sZW5ndGggPiAxID8gbXVsdGlLZXlQYXRoIDoga2V5UGF0aDtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHJldHVybiB1bmRlZmluZWQ7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBwcml2YXRlIHN0YXRpYyBhZGROZXdTdG9yZSh1cGdyYWRlREI6IElEQlBEYXRhYmFzZSwgc3RvcmU6IElPYmplY3RTdG9yZSkge1xuICAgICAgICB0cnkge1xuXG4gICAgICAgICAgICBjb25zdCBuZXdTdG9yZSA9IHVwZ3JhZGVEQi5jcmVhdGVPYmplY3RTdG9yZShzdG9yZS5uYW1lLFxuICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAga2V5UGF0aDogdGhpcy5nZXRLZXlQYXRoKHN0b3JlLmtleVBhdGgpLFxuICAgICAgICAgICAgICAgICAgICBhdXRvSW5jcmVtZW50OiBzdG9yZS5hdXRvSW5jcmVtZW50XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgKTtcblxuICAgICAgICAgICAgZm9yICh2YXIgaW5kZXggb2Ygc3RvcmUuaW5kZXhlcykge1xuICAgICAgICAgICAgICAgIHRyeSB7XG5cbiAgICAgICAgICAgICAgICAgICAgbmV3U3RvcmUuY3JlYXRlSW5kZXgoaW5kZXgubmFtZSxcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuZ2V0S2V5UGF0aChpbmRleC5rZXlQYXRoKSA/PyBpbmRleC5uYW1lLFxuICAgICAgICAgICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG11bHRpRW50cnk6IGluZGV4Lm11bHRpRW50cnksXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdW5pcXVlOiBpbmRleC51bmlxdWVcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgICAgICAgICB0aHJvdyBgaW5kZXggJHtpbmRleC5uYW1lfSwgJHtlcnJvci50b1N0cmluZygpfWA7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdGhyb3cgYHN0b3JlICR7c3RvcmUubmFtZX0sICR7ZXJyb3IudG9TdHJpbmcoKX1gO1xuICAgICAgICB9XG4gICAgfVxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdGlmKF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0pIHtcblx0XHRyZXR1cm4gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiLy8gZGVmaW5lIGdldHRlciBmdW5jdGlvbnMgZm9yIGhhcm1vbnkgZXhwb3J0c1xuX193ZWJwYWNrX3JlcXVpcmVfXy5kID0gKGV4cG9ydHMsIGRlZmluaXRpb24pID0+IHtcblx0Zm9yKHZhciBrZXkgaW4gZGVmaW5pdGlvbikge1xuXHRcdGlmKF9fd2VicGFja19yZXF1aXJlX18ubyhkZWZpbml0aW9uLCBrZXkpICYmICFfX3dlYnBhY2tfcmVxdWlyZV9fLm8oZXhwb3J0cywga2V5KSkge1xuXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIGtleSwgeyBlbnVtZXJhYmxlOiB0cnVlLCBnZXQ6IGRlZmluaXRpb25ba2V5XSB9KTtcblx0XHR9XG5cdH1cbn07IiwiX193ZWJwYWNrX3JlcXVpcmVfXy5vID0gKG9iaiwgcHJvcCkgPT4gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsKG9iaiwgcHJvcCkiLCIvLyBkZWZpbmUgX19lc01vZHVsZSBvbiBleHBvcnRzXG5fX3dlYnBhY2tfcmVxdWlyZV9fLnIgPSAoZXhwb3J0cykgPT4ge1xuXHRpZih0eXBlb2YgU3ltYm9sICE9PSAndW5kZWZpbmVkJyAmJiBTeW1ib2wudG9TdHJpbmdUYWcpIHtcblx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgU3ltYm9sLnRvU3RyaW5nVGFnLCB7IHZhbHVlOiAnTW9kdWxlJyB9KTtcblx0fVxuXHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgJ19fZXNNb2R1bGUnLCB7IHZhbHVlOiB0cnVlIH0pO1xufTsiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZVxuX193ZWJwYWNrX3JlcXVpcmVfXyhcIi4vY2xpZW50L0luaXRpYWxpc2VJbmRleERiQmxhem9yLnRzXCIpO1xuLy8gVGhpcyBlbnRyeSBtb2R1bGUgdXNlZCAnZXhwb3J0cycgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxuIl0sInNvdXJjZVJvb3QiOiIifQ==